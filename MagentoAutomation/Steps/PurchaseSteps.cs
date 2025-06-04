using MagentoTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace MagentoTests.Steps;

[Binding]
public class PurchaseSteps
{
    private readonly IWebDriver _driver;
    private readonly LoginPage _loginPage;
    private readonly ProductPage _productPage;
    private readonly CartPage _cartPage;
    private readonly CheckoutPage _checkoutPage;
    private readonly OrderPage _orderPage;
    private static readonly string ReportPath;

    static PurchaseSteps()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        ReportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", $"test-report_{timestamp}.txt");
    }

    public PurchaseSteps()
    {
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        
        var chromeOptions = new ChromeOptions();
        
        bool isHeadless = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HEADLESS")) && 
                          Environment.GetEnvironmentVariable("HEADLESS").ToLower() == "true";
        
        if (isHeadless)
        {
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            LogToFile("Running in headless mode for CI/CD");
        }
        
        chromeOptions.AddArgument("--start-maximized");
        chromeOptions.AddArgument("--disable-extensions");
        chromeOptions.AddArgument("--disable-popup-blocking");
        chromeOptions.AddArgument("--disable-infobars");
        
        _driver = new ChromeDriver(chromeOptions);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        
        _loginPage = new LoginPage(_driver);
        _productPage = new ProductPage(_driver);
        _cartPage = new CartPage(_driver);
        _checkoutPage = new CheckoutPage(_driver);
        _orderPage = new OrderPage(_driver);

        if (!Directory.Exists("Reports"))
            Directory.CreateDirectory("Reports");
        
        File.WriteAllText(ReportPath, $"Test Run: {DateTime.Now}\n\n");
    }

    private void LogToFile(string message)
    {
        File.AppendAllText(ReportPath, $"{DateTime.Now}: {message}\n");
        
        Console.WriteLine($"{DateTime.Now}: {message}");
    }

    [Given(@"I am logged in as a registered user")]
    public void GivenIAmLoggedIn()
    {

        _loginPage.Login("vzp72382@jioso.com", "vzp72382@jioso");
        LogToFile("Logged in successfully");
    }

    [When(@"I add 2 jackets and 1 pants to the cart")]
    public void WhenIAddItemsToCart()
    {
        _productPage.AddJacketsToCart(2);
        
        LogToFile("Added 2 jackets and 1 pants to cart");
    }
    
    [When(@"I proceed to checkout")]
    public void WhenIProceedToCheckout()
    {
        _cartPage.ProceedToCheckout();
        LogToFile("Proceeded to checkout");
    }

    [Then(@"I verify the order summary")]
    public void ThenIVerifyOrderSummary()
    {
        _checkoutPage.VerifyOrderSummary();
        LogToFile("Order summary verified");
    }

    [When(@"I enter a valid delivery address")]
    public void WhenIEnterDeliveryAddress()
    {
        _checkoutPage.EnterDeliveryAddress();
        LogToFile("Entered delivery address");
    }

    [When(@"I select a delivery method")]
    public void WhenISelectDeliveryMethod()
    {
        _checkoutPage.SelectDeliveryMethod();
        LogToFile("Selected delivery method");
    }

    [When(@"I place the order")]
    public void WhenIPlaceOrder()
    {
        _checkoutPage.PlaceOrder();
        LogToFile("Order placed successfully");
    }

    [Then(@"I verify the order in My Orders")]
    public void ThenIVerifyOrderInMyOrders()
    {
        _orderPage.VerifyOrder();
        LogToFile("Order verified in My Orders");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        LogToFile("Test completed\n");
        
        try
        {
            try {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                var scenarioName = ScenarioContext.Current.ScenarioInfo.Title.Replace(" ", "_");
                var fileName = $"final_{scenarioName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                LogToFile($"Final screenshot saved as {fileName}");
            }
            catch (Exception ex) {
                LogToFile($"Error taking final screenshot: {ex.Message}");
            }
            
            _driver.Quit();
        }
        catch (Exception ex)
        {
            LogToFile($"Error during driver cleanup: {ex.Message}");
        }
    }
}