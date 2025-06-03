using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
namespace MagentoTests.Pages;

public class OrderPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public OrderPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    private By orderItems = By.CssSelector("table.data.table.orders tbody tr");
    private By orderIdColumn = By.CssSelector("td.col.id");
    private By orderDateColumn = By.CssSelector("td.col.date");

    public void VerifyOrder()
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/sales/order/history/");

        Console.WriteLine($"Current URL: {_driver.Url}");

        _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));


        var orders = _driver.FindElements(orderItems);

        Assert.That(orders.Count, Is.GreaterThan(0), "No orders found in order history");

        Console.WriteLine($"Found {orders.Count} orders in order history");

        var firstOrder = orders.First();
        var orderId = firstOrder.FindElement(orderIdColumn).Text;
        var orderDate = firstOrder.FindElement(orderDateColumn).Text;

        Console.WriteLine($"Found order #{orderId} placed on {orderDate}");

        Assert.That(orderId, Is.Not.Empty, "Order ID is empty");
    }
}