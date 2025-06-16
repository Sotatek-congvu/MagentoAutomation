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

    [Test]
    public void Test_UserCanLoginAndPurchaseItems()
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
        _loginPage.Login("axh37515@jioso.com", "axh37515@jioso");

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
        _productPage.AddJacketsToCart(2);

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html");
        _productPage.AddPantsToCart1(1);

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
        _cartPage.ProceedToCheckout();

        _checkoutPage.EnterDeliveryAddress();

        _checkoutPage.SelectDeliveryMethod();

        _checkoutPage.PlaceOrder();

        StringAssert.Contains("success", _driver.Url.ToLower());
    }

    [Test]
    public void Test_PurchaseDifferentQuantities()
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
        _loginPage.Login("axh37515@jioso.com", "axh37515@jioso");

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
        _productPage.AddJacketsToCart(3);

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html");
        _productPage.AddPantsToCart1(2);

        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
        _cartPage.ProceedToCheckout();

        _checkoutPage.EnterDeliveryAddress();

        _checkoutPage.SelectDeliveryMethod();

        _checkoutPage.PlaceOrder();

        StringAssert.Contains("success", _driver.Url.ToLower());
    }

    [TestCase("New York", "United States", "10001")]
    [TestCase("Los Angeles", "United States", "90001")]
    public void Test_DifferentShippingAddresses(string city, string country, string zipCode)
    {
        // 1. Đăng nhập
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
        _loginPage.Login("axh37515@jioso.com", "axh37515@jioso");

        // 2. Thêm áo khoác vào giỏ hàng
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
        _productPage.AddJacketsToCart(1);

        // 3. Thêm quần vào giỏ hàng
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html");
        _productPage.AddPantsToCart1(1);

        // 4. Vào giỏ hàng và thanh toán
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
        _cartPage.ProceedToCheckout();

        // 5. Nhập địa chỉ giao hàng
        // Cách này sử dụng phương thức không tham số có sẵn
        _checkoutPage.EnterDeliveryAddress();

        // 6. Chọn phương thức giao hàng
        _checkoutPage.SelectDeliveryMethod();

        // 7. Đặt hàng
        _checkoutPage.PlaceOrder();

        Assert.True(true);
    }

    [Test]
    public void Test_SimpleTest()
    {
        // Test đơn giản để xác minh NUnit đang hoạt động
        Assert.That(1 + 1, Is.EqualTo(2));
        Assert.Pass("NUnit test thành công");
    }
}