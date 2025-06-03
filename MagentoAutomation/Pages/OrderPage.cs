using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
namespace MagentoTests.Pages;

public class OrderPage
{
    private readonly IWebDriver _driver;

    public OrderPage(IWebDriver driver) => _driver = driver;

    private IWebElement MyOrders => _driver.FindElement(By.CssSelector(".orders-history"));
    private IWebElement OrderItem => _driver.FindElement(By.CssSelector(".order-item"));

    public void VerifyOrder()
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/sales/order/history/");
        new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(d => MyOrders.Displayed);
        Assert.IsTrue(OrderItem.Displayed, "Order not found in order history");
    }
}
