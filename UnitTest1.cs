using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools.V132.Browser;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Security.Authentication;


namespace epiasAutomation
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var options = new ChromeOptions();
            options.DebuggerAddress = "127.0.0.1:9222";  // local debug port

            using var driver = new ChromeDriver(options); // attach to existing Chrome instance
            
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            // set download path with DevTools
            string downloadPath = Path.Combine(Environment.CurrentDirectory, "Downloads");
            Directory.CreateDirectory(downloadPath);

            var devTools = driver.GetDevToolsSession();
            
            await devTools.SendCommand(new SetDownloadBehaviorCommandSettings()
            {
                Behavior = "allow",
                DownloadPath = downloadPath
            });

            // click through the webstite to download the file
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            var lockButton = wait.Until(d => d.FindElement(By.ClassName("lock-button")));
            lockButton.Click();

            var elektrikItem = wait.Until(d => d.FindElement(By.XPath("//span[@class='item-title' and contains(., 'ELEKTRİK')]")));
            elektrikItem.Click();

            var elektrikPiyasaItem = wait.Until(d => d.FindElement(By.XPath("//span[@class='item-title' and normalize-space()='ELEKTRİK PİYASALARI']")));
            elektrikPiyasaItem.Click();

            var gopItem = wait.Until(d => d.FindElement(By.XPath("//span[@class='item-title' and normalize-space()='Gün Öncesi Piyasası (GÖP)']")));
            gopItem.Click();

            var ptfItem = wait.Until(d => d.FindElement(By.XPath("//span[@class='item-title' and normalize-space()='Piyasa Takas Fiyatı (PTF)']")));
            ptfItem.Click();

            var yesterday = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");

            var dateInput = wait.Until(d => d.FindElement(By.Name("startDate")));

            dateInput.SendKeys(Keys.Control + "a");
            dateInput.SendKeys(Keys.Delete);

            dateInput.SendKeys(yesterday);
            
            var queryButton = wait.Until(d => d.FindElement(By.CssSelector("button.epui-button.primary-btn.btn-xs")));
            queryButton.Click();

            var exportButton = wait.Until(d => d.FindElement(By.CssSelector("div.epuitable-toolbar-right-side-export")));
            exportButton.Click();

            var xlsxOption = wait.Until(d => d.FindElement(By.XPath("//div[@class='epuitable-export-panel-item']//span[normalize-space()='XLSX Formatında']")));
            xlsxOption.Click();

            // wait for file to download
            var fileWaiter = new DefaultWait<string>(downloadPath)
            {
                Timeout = TimeSpan.FromSeconds(30),
                PollingInterval = TimeSpan.FromSeconds(1)
            };
            string downloadedFile = fileWaiter.Until(dir => 
                Directory.GetFiles(dir, "*.xlsx").FirstOrDefault()
            );


            // read credentials
            string? projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName; // go up 3 directories
            string configPath = Path.Combine(projectRoot, "appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false)
                .Build();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EPİAŞ Automation", config["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("Recipient", config["EmailSettings:RecipientEmail"]));
            message.Subject = $"PTF Report - {yesterday}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = "Attached is the requested PTF report.";
            bodyBuilder.Attachments.Add(downloadedFile);

            message.Body = bodyBuilder.ToMessageBody();

            // send email
            try
            {
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                config["EmailSettings:SmtpServer"], 
                int.Parse(config["EmailSettings:SmtpPort"]), 
                MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable // fallback
            );

            await smtpClient.AuthenticateAsync(
                config["EmailSettings:SenderEmail"],
                config["EmailSettings:SenderPassword"] // app password
            );

            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
            }
            driver.Close();
        }
    }
}