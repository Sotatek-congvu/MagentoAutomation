namespace FileTest;

using MagentoTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Kiểm thử tự động cho quy trình mua hàng trên Magento
/// </summary>
public class MagentoPurchaseTests : IAsyncLifetime
{
    private IWebDriver _driver;
    private LoginPage _loginPage;
    private ProductPage _productPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;
    private OrderPage _orderPage;
    private readonly string _reportDirectory;
    private readonly string _reportPath;
    private readonly string _screenshotsDirectory;

    public MagentoPurchaseTests()
    {
        // Tạo thư mục báo cáo và ảnh chụp màn hình
        _reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
        _screenshotsDirectory = Path.Combine(_reportDirectory, "Screenshots");

        if (!Directory.Exists(_reportDirectory))
            Directory.CreateDirectory(_reportDirectory);

        if (!Directory.Exists(_screenshotsDirectory))
            Directory.CreateDirectory(_screenshotsDirectory);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _reportPath = Path.Combine(_reportDirectory, $"test_report_{timestamp}.txt");

        // Khởi tạo file báo cáo
        File.WriteAllText(_reportPath, $"Test Run Started: {DateTime.Now}\n\n");
    }

    public async Task InitializeAsync()
    {
        LogInfo("Khởi tạo trình duyệt");

        try
        {
            // Thiết lập Firefox options
            var options = new FirefoxOptions();

            // Kiểm tra xem có chạy trong CI/CD không
            bool isHeadless = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

            if (isHeadless)
            {
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-gpu");
                LogInfo("Chạy trong chế độ headless cho CI/CD");
            }

            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-infobars");

            // Khởi tạo driver
            _driver = new FirefoxDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0); // Sử dụng explicit wait thay vì implicit wait

            // Khởi tạo các page object
            _loginPage = new LoginPage(_driver);
            _productPage = new ProductPage(_driver);
            _cartPage = new CartPage(_driver);
            _checkoutPage = new CheckoutPage(_driver);
            _orderPage = new OrderPage(_driver);

            // Mở trang web Magento
            _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/");
            LogInfo("Đã mở trang web Magento thành công");
        }
        catch (Exception ex)
        {
            LogError($"Lỗi khởi tạo: {ex.Message}");
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            LogInfo("Kết thúc kiểm thử, đóng trình duyệt");
            _driver?.Quit();
        }
        catch (Exception ex)
        {
            LogError($"Lỗi khi đóng trình duyệt: {ex.Message}");
        }
    }

    [Fact]
    public void Test_UserCanLoginAndPurchaseItems()
    {
        string testName = "Đăng nhập và mua hàng";
        LogTestStart(testName);

        try
        {
            // 1. Đăng nhập
            LoginUser("axh37515@jioso.com", "axh37515@jioso");

            // 2. Thêm sản phẩm vào giỏ hàng
            AddProductsToCart(2, 1);

            // 3. Thanh toán
            CompleteCheckoutProcess();

            // 4. Xác thực đơn hàng
            VerifyOrderSuccess();

            LogTestEnd(testName, true);
        }
        catch (Exception ex)
        {
            LogTestEnd(testName, false, ex.Message);
            TakeScreenshot($"error_{testName.Replace(" ", "_")}");
            throw;
        }
    }

    [Fact]
    public void Test_UserCanPurchaseMultipleProducts()
    {
        string testName = "Mua nhiều sản phẩm khác nhau";
        LogTestStart(testName);

        try
        {
            // 1. Đăng nhập
            LoginUser("axh37515@jioso.com", "axh37515@jioso");

            // 2. Thêm nhiều sản phẩm vào giỏ hàng
            AddProductsToCart(3, 2);

            // 3. Thanh toán
            CompleteCheckoutProcess();

            // 4. Xác thực đơn hàng
            VerifyOrderSuccess();

            LogTestEnd(testName, true);
        }
        catch (Exception ex)
        {
            LogTestEnd(testName, false, ex.Message);
            TakeScreenshot($"error_{testName.Replace(" ", "_")}");
            throw;
        }
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(1, 2)]
    public void Test_PurchaseVariousQuantities(int jacketQuantity, int pantsQuantity)
    {
        string testName = $"Mua {jacketQuantity} áo và {pantsQuantity} quần";
        LogTestStart(testName);

        try
        {
            // 1. Đăng nhập
            LoginUser("axh37515@jioso.com", "axh37515@jioso");

            // 2. Thêm sản phẩm vào giỏ hàng
            AddProductsToCart(jacketQuantity, pantsQuantity);

            // 3. Thanh toán
            CompleteCheckoutProcess();

            // 4. Xác thực đơn hàng
            VerifyOrderSuccess();

            LogTestEnd(testName, true);
        }
        catch (Exception ex)
        {
            LogTestEnd(testName, false, ex.Message);
            TakeScreenshot($"error_quantity_{jacketQuantity}_{pantsQuantity}");
            throw;
        }
    }

    /// <summary>
    /// Đăng nhập vào hệ thống
    /// </summary>
    private void LoginUser(string email, string password)
    {
        LogStep("Đăng nhập vào hệ thống");
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");
        _loginPage.Login(email, password);
        LogInfo($"Đăng nhập thành công với tài khoản {email}");
        TakeScreenshot("after_login");
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    private void AddProductsToCart(int jacketQuantity, int pantsQuantity)
    {
        try
        {
            LogStep($"Thêm {jacketQuantity} áo khoác vào giỏ hàng");
            _productPage.AddJacketsToCart(jacketQuantity);
            TakeScreenshot("after_add_jackets");

            LogStep($"Thêm {pantsQuantity} quần vào giỏ hàng");
            _productPage.AddPantsToCart1(pantsQuantity);
            TakeScreenshot("after_add_pants");

            LogInfo($"Đã thêm tổng cộng {jacketQuantity + pantsQuantity} sản phẩm vào giỏ hàng");
        }
        catch (Exception ex)
        {
            LogError($"Lỗi khi thêm sản phẩm: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Hoàn thành quá trình thanh toán
    /// </summary>
    private void CompleteCheckoutProcess()
    {
        try
        {
            LogStep("Chuyển đến trang giỏ hàng");
            _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
            TakeScreenshot("cart_page");

            LogStep("Tiến hành thanh toán");
            _cartPage.ProceedToCheckout();

            LogStep("Nhập địa chỉ giao hàng");
            _checkoutPage.EnterDeliveryAddress();
            TakeScreenshot("delivery_address");

            LogStep("Chọn phương thức giao hàng");
            _checkoutPage.SelectDeliveryMethod();
            TakeScreenshot("shipping_method");

            LogStep("Đặt hàng");
            _checkoutPage.PlaceOrder();
            TakeScreenshot("order_placed");
        }
        catch (Exception ex)
        {
            LogError($"Lỗi trong quá trình thanh toán: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Xác thực đơn hàng đã đặt thành công
    /// </summary>
    private void VerifyOrderSuccess()
    {
        LogStep("Xác thực đơn hàng thành công");

        try
        {

            var pageTitle = _driver.FindElement(By.CssSelector(".page-title .base")).Text;
            Assert.That(pageTitle, Does.Contain("Thank you").IgnoreCase);

            LogInfo($"Đơn hàng đã được đặt thành công: '{pageTitle}'");
        }
        catch (Exception ex)
        {
            LogError($"Không thể xác thực đơn hàng: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Ghi thông tin vào log
    /// </summary>
    private void LogInfo(string message)
    {
        string logMessage = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_reportPath, logMessage + "\n");
    }

    /// <summary>
    /// Ghi lỗi vào log
    /// </summary>
    private void LogError(string message)
    {
        string logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_reportPath, logMessage + "\n");
    }

    /// <summary>
    /// Ghi bước thực hiện vào log
    /// </summary>
    private void LogStep(string step)
    {
        string logMessage = $"[STEP] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {step}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_reportPath, logMessage + "\n");
    }

    /// <summary>
    /// Đánh dấu bắt đầu test case
    /// </summary>
    private void LogTestStart(string testName)
    {
        string logMessage = $"[TEST START] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {testName}";
        Console.WriteLine(new string('=', 80));
        Console.WriteLine(logMessage);
        Console.WriteLine(new string('-', 80));

        File.AppendAllText(_reportPath, "\n" + new string('=', 80) + "\n");
        File.AppendAllText(_reportPath, logMessage + "\n");
        File.AppendAllText(_reportPath, new string('-', 80) + "\n");
    }

    /// <summary>
    /// Đánh dấu kết thúc test case
    /// </summary>
    private void LogTestEnd(string testName, bool success, string error = null)
    {
        string result = success ? "PASSED" : "FAILED";
        string logMessage = $"[TEST {result}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {testName}";
        if (!string.IsNullOrEmpty(error))
        {
            logMessage += $"\nError: {error}";
        }

        Console.WriteLine(new string('-', 80));
        Console.WriteLine(logMessage);
        Console.WriteLine(new string('=', 80));

        File.AppendAllText(_reportPath, new string('-', 80) + "\n");
        File.AppendAllText(_reportPath, logMessage + "\n");
        File.AppendAllText(_reportPath, new string('=', 80) + "\n\n");
    }

    /// <summary>
    /// Chụp ảnh màn hình
    /// </summary>
    private void TakeScreenshot(string name)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            var filename = Path.Combine(_screenshotsDirectory, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            screenshot.SaveAsFile(filename);
            LogInfo($"Đã chụp ảnh màn hình: {filename}");
        }
        catch (Exception ex)
        {
            LogError($"Lỗi khi chụp ảnh màn hình: {ex.Message}");
        }
    }
}