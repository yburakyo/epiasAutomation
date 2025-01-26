using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace epiasAutomation
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var options = new ChromeOptions();
            options.DebuggerAddress = "127.0.0.1:9222";  // Local debug port
            
            // Attach to existing Chrome instance
            using var driver = new ChromeDriver(options);
            
            driver.SwitchTo().Window(driver.WindowHandles[0]);


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

            dateInput.SendKeys(Keys.Control + "a"); // Select all
            dateInput.SendKeys(Keys.Delete); // Delete selection

            dateInput.SendKeys(yesterday);
            
            var queryButton = wait.Until(d => d.FindElement(By.CssSelector("button.epui-button.primary-btn.btn-xs")));
            queryButton.Click();

            var exportButton = wait.Until(d => d.FindElement(By.CssSelector("div.epuitable-toolbar-right-side-export")));
            exportButton.Click();

            var xlsxOption = wait.Until(d => d.FindElement(By.XPath("//div[@class='epuitable-export-panel-item']//span[normalize-space()='XLSX Formatında']")));
            xlsxOption.Click();

            System.Threading.Thread.Sleep(5000); // sleep to see the website, will remove later

            driver.Close();  // Closes Selenium connection but leaves Chrome running
        }
    }
}