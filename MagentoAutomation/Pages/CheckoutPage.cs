using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace MagentoTests.Pages;

public class CheckoutPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public CheckoutPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
    }

    private By orderSummaryContainer = By.CssSelector(".opc-block-summary");
    private By orderItems = By.CssSelector(".items-in-cart .product-item-name");
    private By orderPrices = By.CssSelector(".items-in-cart .price");

    private By emailField = By.Id("customer-email");
    private By firstNameField = By.Name("firstname");
    private By lastNameField = By.Name("lastname");
    private By streetAddressField = By.Name("street[0]");
    private By cityField = By.Name("city");
    private By stateDropdown = By.Name("region_id");
    private By zipCodeField = By.Name("postcode");
    private By countryDropdown = By.Name("country_id");
    private By phoneField = By.Name("telephone");

    private By flatRateShipping = By.CssSelector("input[value='flatrate_flatrate']");
    private By shippingMethodNextButton = By.CssSelector(".button.action.continue.primary");

    private By placeOrderButton = By.CssSelector(".action.primary.checkout");
    private By successMessage = By.CssSelector(".page-title .base");

    public void VerifyOrderSummary()
    {
        _wait.Until(d => d.FindElement(orderSummaryContainer));

        var items = _driver.FindElements(orderItems);
        var prices = _driver.FindElements(orderPrices);

        Assert.That(items.Count, Is.GreaterThanOrEqualTo(3), "Expected at least 3 items in cart");



    }

    public void EnterDeliveryAddress()
    {
        _wait.Until(d => d.FindElement(firstNameField));

        if (_driver.FindElements(emailField).Count > 0 && _driver.FindElement(emailField).Displayed)
        {
            _driver.FindElement(emailField).SendKeys("testuser@example.com");
        }

        _driver.FindElement(firstNameField).SendKeys("Test");
        _driver.FindElement(lastNameField).SendKeys("User");
        _driver.FindElement(streetAddressField).SendKeys("123 Test Street");
        _driver.FindElement(cityField).SendKeys("Test City");

        var stateSelect = new SelectElement(_driver.FindElement(stateDropdown));
        stateSelect.SelectByText("California");

        var countrySelect = new SelectElement(_driver.FindElement(countryDropdown));
        countrySelect.SelectByText("United States");

        _driver.FindElement(zipCodeField).SendKeys("90210");
        _driver.FindElement(phoneField).SendKeys("1234567890");


    }

    public void SelectDeliveryMethod()
    {
        _wait.Until(d => d.FindElement(flatRateShipping));

        _driver.FindElement(flatRateShipping).Click();

        _wait.Until(d => d.FindElement(shippingMethodNextButton)).Click();

        var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
        screenshot.SaveAsFile("shipping-method.png");

        _wait.Until(d => d.FindElement(placeOrderButton));
    }


    public void PlaceOrder()
    {
        if (!_driver.Url.Contains("checkout"))
        {
            Console.WriteLine("Not on checkout page, navigating back.");
            _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/");
            _wait.Until(d => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        _wait.Until(d => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));

        try
        {
            var modals = _driver.FindElements(By.CssSelector(".modal-popup, .loading-mask, .minicart-wrapper.active"));
            foreach (var modal in modals)
            {
                try
                {
                    if (modal.Displayed)
                    {
                        var closeButton = _driver.FindElement(By.CssSelector(".action.close"));
                        if (closeButton != null && closeButton.Displayed)
                        {
                            closeButton.Click();
                            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".modal-popup, .loading-mask, .minicart-wrapper.active")));
                        }
                    }
                }
                catch (StaleElementReferenceException)
                {
                    Console.WriteLine("Modal element became stale, continuing execution");
                }
                catch (NoSuchElementException)
                {
                    // Không tìm thấy nút đóng, bỏ qua
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling modals: {ex.Message}");
        }

        try
        {
            var overlaySelectors = new[] {
            ".checkout-billing-address",
            ".modal-popup",
            ".loading-mask",
            ".minicart-wrapper.active"
        };

            foreach (var selector in overlaySelectors)
            {
                try
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript(@"
                    var elements = document.querySelectorAll(arguments[0]);
                    for (var i = 0; i < elements.length; i++) {
                        if (elements[i].offsetParent !== null) {
                            elements[i].style.display = 'none';
                        }
                    }
                ", selector);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling overlay {selector}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling overlays: {ex.Message}");
        }

        try
        {
            _wait.Until(d => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));

            var paymentMethods = _driver.FindElements(By.CssSelector("input[name='payment[method]']"));
            if (paymentMethods.Any())
            {
                // Sử dụng JavaScript để chọn phương thức thanh toán đầu tiên
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].click(); arguments[0].checked = true;",
                    paymentMethods.First());

                Thread.Sleep(2000); // Đợi để trang phản hồi

                // Tìm và click nút Apply nếu có
                var applyButtons = _driver.FindElements(By.CssSelector(".payment-method-content .action.primary"));
                if (applyButtons.Any() && applyButtons.First().Displayed && applyButtons.First().Enabled)
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", applyButtons.First());
                    Thread.Sleep(1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting payment method: {ex.Message}");
        }

        try
        {
            bool clickSuccess = (bool)((IJavaScriptExecutor)_driver).ExecuteScript(@"
            try {
                // Tìm nút đặt hàng
                let placeOrderButton = document.querySelector('.action.primary.checkout');
                if (placeOrderButton && placeOrderButton.offsetParent !== null) {
                    // Scroll đến nút
                    placeOrderButton.scrollIntoView({block: 'center'});
                    
                    // Chờ một chút để trang hiển thị nút
                    setTimeout(function() {}, 500);
                    
                    // Click vào nút
                    placeOrderButton.click();
                    return true;
                }
                return false;
            } catch (e) {
                console.error('Error clicking button:', e);
                return false;
            }
        ");

            if (!clickSuccess)
            {
                Console.WriteLine("JavaScript click failed, trying WebDriver click");

                var orderButton = _driver.FindElement(placeOrderButton);
                _wait.Until(ExpectedConditions.ElementToBeClickable(placeOrderButton));
                orderButton.Click();
            }

            var attemptScreenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            attemptScreenshot.SaveAsFile("place-order-attempt.png");

            
            bool redirectSuccess = false;
            int attempts = 0;
            while (!redirectSuccess && attempts < 3)
            {
                try
                {
                    _wait.Until(d => d.Url.Contains("checkout/onepage/success"));
                    redirectSuccess = true;
                }
                catch (WebDriverTimeoutException)
                {
                    attempts++;
                    if (attempts >= 3)
                    {
                        throw;
                    }
                    Thread.Sleep(1000);
                }
            }
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine("Timeout waiting for success page, checking for errors");

            var debugScreenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            debugScreenshot.SaveAsFile($"order-failure-{DateTime.Now:yyyyMMdd_HHmmss}.png");

            // Kiểm tra nếu đã chuyển hướng thành công nhưng timeout xảy ra
            if (_driver.Url.Contains("checkout/onepage/success"))
            {
                Console.WriteLine("Success page URL detected despite timeout");
            }
            else
            {
                // Kiểm tra lỗi
                var errorModals = _driver.FindElements(By.CssSelector(".modal-content, .message-error"));
                if (errorModals.Any())
                {
                    throw new Exception($"Order failed: {errorModals.First().Text}");
                }
                throw new Exception("Order placement failed, no success page loaded.");
            }
        }

        // Xác nhận đặt hàng thành công
        try
        {
            _wait.Until(ExpectedConditions.ElementExists(successMessage));
            _wait.Until(d => d.FindElement(successMessage).Displayed);

            string pageTitle = _driver.FindElement(successMessage).Text;
            Assert.That(pageTitle, Does.Contain("Thank you"), "Order success message not found");

            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            screenshot.SaveAsFile("order-success.png");

            Console.WriteLine("Order placed successfully, confirmation message found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying success message: {ex.Message}");
            throw;
        }
    }

}

