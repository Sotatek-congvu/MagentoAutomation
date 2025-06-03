using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MagentoTests.Pages;

public class LoginPage
{
    private readonly IWebDriver _driver;

    public LoginPage(IWebDriver driver) => _driver = driver;

    private IWebElement EmailField => _driver.FindElement(By.Id("email"));
    private IWebElement PasswordField => _driver.FindElement(By.Id("pass"));
    private IWebElement LoginButton => _driver.FindElement(By.Id("send2"));
    private IWebElement WelcomeMessage => _driver.FindElement(By.CssSelector(".welcome"));

    public void Login(string email, string password)
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/referer/aHR0cHM6Ly9tYWdlbnRvLnNvZnR3YXJldGVzdGluZ2JvYXJkLmNvbS8%2C/");
        EmailField.SendKeys(email);
        PasswordField.SendKeys(password);
        LoginButton.Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(d => WelcomeMessage.Displayed);
    }
}