using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace MagentoTests.Pages;

public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    private By EmailFieldLocator => By.Id("email");
    private By PasswordFieldLocator => By.Id("pass");
    private By LoginButtonLocator => By.Id("send2");
    private By WelcomeMessageLocator => By.CssSelector(".greet.welcome");

    public void Login(string email, string password)
    {
        try
        {
            Console.WriteLine($"Logging in as {email}");
            _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
            _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            
            // Đợi cho trang đăng nhập tải xong và field hiển thị
            var emailField = _wait.Until(ExpectedConditions.ElementToBeClickable(EmailFieldLocator));
            emailField.Clear();
            emailField.SendKeys(email);
            
            var passwordField = _wait.Until(ExpectedConditions.ElementToBeClickable(PasswordFieldLocator));
            passwordField.Clear();
            passwordField.SendKeys(password);
            
            var loginButton = _wait.Until(ExpectedConditions.ElementToBeClickable(LoginButtonLocator));
            loginButton.Click();
            
            try
            {
                _wait.Until(ExpectedConditions.ElementIsVisible(WelcomeMessageLocator));
                var welcomeMessage = _driver.FindElement(WelcomeMessageLocator);
                Console.WriteLine($"Login successful: {welcomeMessage.Text}");
            }
            catch (WebDriverTimeoutException)
            {
                // Kiểm tra các lỗi đăng nhập
                var errorMessages = _driver.FindElements(By.CssSelector(".message-error"));
                if (errorMessages.Count > 0)
                {
                    throw new Exception($"Login failed: {errorMessages[0].Text}");
                }
                
                // Nếu không thấy thông báo lỗi nhưng cũng không thấy welcome message
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                var filename = $"login-error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                screenshot.SaveAsFile(filename);
                Console.WriteLine($"Login error screenshot saved as {filename}");
                
                throw new Exception("Login failed: Could not verify welcome message after login");
            }
        }
        catch (Exception ex)
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            var filename = $"login-exception_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            screenshot.SaveAsFile(filename);
            Console.WriteLine($"Exception during login: {ex.Message}");
            Console.WriteLine($"Login exception screenshot saved as {filename}");
            throw;
        }
    }
}