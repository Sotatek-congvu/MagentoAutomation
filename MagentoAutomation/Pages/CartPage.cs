using OpenQA.Selenium;

namespace MagentoTests.Pages;

public class CartPage
{
    private readonly IWebDriver _driver;

    public CartPage(IWebDriver driver) => _driver = driver;

    private IWebElement CheckoutButton => _driver.FindElement(By.CssSelector(".checkout-methods-items button.action.primary.checkout"));

    public void ProceedToCheckout()
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/checkout/cart/");
        CheckoutButton.Click();
    }
}