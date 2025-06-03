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
        
        Assert.That(items.Count, Is.GreaterThanOrEqualTo(2), "Expected at least 2 items in cart");
        
        
       
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

        var modals = _driver.FindElements(By.CssSelector(".modal-popup, .loading-mask, .minicart-wrapper.active"));
        foreach (var modal in modals)
        {
            if (modal.Displayed)
            {
                try
                {
                    _driver.FindElement(By.CssSelector(".action.close")).Click();
                    _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".modal-popup, .loading-mask, .minicart-wrapper.active")));
                }
                catch (NoSuchElementException) { }
            }
        }

        var overlays = _driver.FindElements(By.CssSelector(".checkout-billing-address, .modal-popup, .loading-mask, .minicart-wrapper.active"));
        foreach (var overlay in overlays)
        {
            if (overlay.Displayed)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].style.display = 'none';", overlay);
            }
        }

        try
        {
            var paymentMethod = _driver.FindElements(By.CssSelector("input[name='payment[method]']"));
            if (paymentMethod.Any() && !paymentMethod.First().Selected)
            {
                paymentMethod.First().Click();
                _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("input[name='payment[method]']")));
                Thread.Sleep(1500); // Đợi DOM ổn định lâu hơn

                int retryCount = 0;
                const int maxRetries = 5; // Tăng số lần thử
                while (retryCount < maxRetries)
                {
                    try
                    {
                        var applyButton = _driver.FindElements(By.CssSelector(".payment-method-content .action.primary"));
                        if (applyButton.Any())
                        {
                            applyButton.First().Click();
                            _wait.Until(ExpectedConditions.ElementToBeClickable(placeOrderButton));
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No Apply button found, proceeding to Place Order.");
                            break;
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        Console.WriteLine($"Stale element detected for applyButton, retrying ({retryCount + 1}/{maxRetries}).");
                        retryCount++;
                        Thread.Sleep(1000);
                        if (retryCount == maxRetries) throw;
                    }
                }
            }
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine($"Payment method not found: {ex.Message}");
        }

        // Nhấp nút Place Order
        try
        {
            var placeOrderButton = _driver.FindElement(this.placeOrderButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", placeOrderButton);
            Thread.Sleep(500);
            _wait.Until(ExpectedConditions.ElementToBeClickable(this.placeOrderButton));
            _wait.Until(d => placeOrderButton.Enabled);
            placeOrderButton.Click();
            _wait.Until(d => d.Url.Contains("checkout/onepage/success"));
        }
        catch (ElementClickInterceptedException)
        {
            Console.WriteLine("Click intercepted, using JavaScript to click.");
            var placeOrderButton = _driver.FindElement(this.placeOrderButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", placeOrderButton);
            _wait.Until(d => d.Url.Contains("checkout/onepage/success"));
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine("Page did not navigate to success, checking for error modals.");
            var errorModals = _driver.FindElements(By.CssSelector(".modal.alert, .message-error"));
            if (errorModals.Any() && errorModals.First().Displayed)
            {
                throw new Exception($"Order failed: {errorModals.First().Text}");
            }
            throw new Exception("Order placement failed, no success page loaded.");
        }

        _wait.Until(d => d.FindElement(successMessage).Displayed);

        string pageTitle = _driver.FindElement(successMessage).Text;
        Assert.That(pageTitle, Does.Contain("Thank you"), "Order success message not found");

        var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
        screenshot.SaveAsFile("order-success.png");
    }

}

