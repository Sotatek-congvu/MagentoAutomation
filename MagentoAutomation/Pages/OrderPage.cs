using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System;
using System.Linq;

namespace MagentoTests.Pages
{
    public class OrderPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public OrderPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // Tăng thời gian chờ
        }

        private By MyOrdersPage => By.CssSelector(".block-order-history, .orders-history, .account-nav .nav.items, .page-title-wrapper");
        private By OrderItems => By.CssSelector("table.data.table.table-order-items tbody tr"); // Sửa bộ chọn
        private By OrderIdColumn => By.CssSelector("td.col.id");
        private By OrderDateColumn => By.CssSelector("td.col.date");
        private By AccountIndicator => By.CssSelector(".customer-name, .logged-in, .account-nav .nav.items");
        private By NoOrdersMessage => By.CssSelector(".message-info, .no-orders, .empty");

        public void VerifyOrder()
        {
            _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/sales/order/history/");
            _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            Console.WriteLine($"Current URL: {_driver.Url}");

            try
            {
                _wait.Until(d => d.FindElement(AccountIndicator).Displayed);
                Console.WriteLine("User is logged in");
            }
            catch (WebDriverTimeoutException)
            {
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("screenshot_login_error.png");
                throw new Exception("User is not logged in on My Orders page. Screenshot saved: screenshot_login_error.png");
            }
            try
            {
                var popupClose = _driver.FindElement(By.CssSelector(".modal-popup .action-close, .action.hide, .close"));
                if (popupClose.Displayed)
                {
                    popupClose.Click();
                    _wait.Until(d => !popupClose.Displayed);
                    Console.WriteLine("Closed popup");
                }
            }
            catch (NoSuchElementException) { }

            try
            {
                _wait.Until(d => d.FindElement(MyOrdersPage).Displayed);
                Console.WriteLine("My Orders page loaded");
            }
            catch (WebDriverTimeoutException)
            {
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("screenshot_my_orders_load_error.png");
                throw new Exception("Không tải được trang My Orders. Screenshot saved: screenshot_my_orders_load_error.png");
            }

            int retryCount = 0;
            while (retryCount < 5) 
            {
                try
                {
                    try
                    {
                        var noOrders = _driver.FindElement(NoOrdersMessage);
                        if (noOrders.Displayed)
                        {
                            Console.WriteLine("No orders message displayed: " + noOrders.Text);
                        }
                    }
                    catch (NoSuchElementException) { }

                    _wait.Until(d => d.FindElements(OrderItems).Count > 0);
                    var orders = _driver.FindElements(OrderItems);
                    Console.WriteLine($"Found {orders.Count} orders in order history");

                    var firstOrder = orders.First();
                    var orderId = firstOrder.FindElement(OrderIdColumn).Text;
                    var orderDate = firstOrder.FindElement(OrderDateColumn).Text;

                    Console.WriteLine($"Found order #{orderId} placed on {orderDate}");

                    Assert.That(orderId, Is.Not.Empty, "Order ID is empty");
                    Console.WriteLine("Verified order in My Orders");
                    return;
                }
                catch (WebDriverTimeoutException)
                {
                    retryCount++;
                    Console.WriteLine($"No order items found, retrying ({retryCount}/5).");
                    _driver.Navigate().Refresh();
                    _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
            }

            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("screenshot_my_orders_error.png");
            throw new Exception($"No orders found in order history after 5 retries\nExpected: greater than 0\nBut was: 0\nScreenshot saved: screenshot_my_orders_error.png");
        }
    }
}