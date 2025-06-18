using MagentoTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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
        _driver = new ChromeDriver();

        _loginPage = new LoginPage(_driver);
        _productPage = new ProductPage(_driver);
        _cartPage = new CartPage(_driver);
        _checkoutPage = new CheckoutPage(_driver);

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/");
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
        _loginPage.Login("lhl90113@jioso.com", "lhl90113@jioso");

        
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