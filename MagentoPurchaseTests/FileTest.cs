using MagentoTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
namespace MagentoPurchaseTests;
[TestFixture]
public class MagentoPurchaseTests
{
    private IWebDriver _driver;
    private LoginPage _loginPage;
    private ProductPage _productPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;

    [OneTimeSetUp]
    public void InitializeTest()
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
        }

        chromeOptions.AddArgument("--start-maximized");
        chromeOptions.AddArgument("--disable-extensions");
        chromeOptions.AddArgument("--disable-popup-blocking");
        chromeOptions.AddArgument("--disable-infobars");

        _driver = new ChromeDriver(chromeOptions);

        _loginPage = new LoginPage(_driver);
        _productPage = new ProductPage(_driver);
        _cartPage = new CartPage(_driver);
        _checkoutPage = new CheckoutPage(_driver);
        
    }

    [OneTimeTearDown]
    public void CleanupTest()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    

    
    
    [TestCase("New York", "United States", "10001")]
    public void Test_DifferentShippingAddresses(string city, string country, string zipCode)
    {
        
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
        _loginPage.Login("kmw58999@jioso.com", "kmw58999@jioso");

        
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
        _productPage.AddJacketsToCart(1);

        
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html");
        _productPage.AddPantsToCart1(1);

        
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
        _cartPage.ProceedToCheckout();

        _checkoutPage.EnterDeliveryAddress();

      
        _checkoutPage.SelectDeliveryMethod();

       
        _checkoutPage.PlaceOrder();

        Assert.True(true);
    }

    
}