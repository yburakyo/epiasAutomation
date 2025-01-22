namespace epiasAutomation;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        IWebDriver driver = new ChromeDriver();

        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("https://seffaflik-prp.epias.com.tr/home");

        System.Threading.Thread.Sleep(2000); // sleep to see the website, will remove later
        driver.Quit();
        
    }
}
