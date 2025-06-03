using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class ProductPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public ProductPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    private By ProductItem(int index) => By.CssSelector($".product-item:nth-child({index + 1})");
    private By SizeOption => By.CssSelector(".swatch-attribute.size .swatch-option:first-child");
    private By ColorOption => By.CssSelector(".swatch-attribute.color .swatch-option:first-child");
    private By AddToCartButton => By.CssSelector(".action.tocart.primary");

    public void AddJacketsToCart(int quantity)
    {
        _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
        _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

        for (int i = 0; i < quantity; i++)
        {
            var productItem = _driver.FindElement(ProductItem(i));
            var addToCartButton = productItem.FindElement(AddToCartButton);

            // Cuộn đến product-item
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", productItem);

            // Hover để hiển thị nút Add to Cart và tùy chọn
            var actions = new Actions(_driver);
            actions.MoveToElement(productItem).Perform();
            Thread.Sleep(1000); // Đợi 1 giây để các tùy chọn hiển thị
            _wait.Until(d => productItem.FindElement(AddToCartButton).Displayed);
            Console.WriteLine($"Add to Cart button displayed for product {i + 1}: {addToCartButton.Displayed}, enabled: {addToCartButton.Enabled}");

            // Xử lý panel bộ lọc
            var filterToggle = _driver.FindElements(By.CssSelector(".filter-options-title"));
            if (filterToggle.Any() && filterToggle.First().Displayed)
            {
                filterToggle.First().Click();
                try
                {
                    _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.filter-options-content")));
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("Filter panel did not collapse, proceeding anyway.");
                }
            }

            // Kiểm tra và chọn kích thước
            try
            {
                var sizeOptions = productItem.FindElements(SizeOption);
                if (!sizeOptions.Any())
                {
                    Console.WriteLine($"No size options for product {i + 1}, skipping.");
                    continue;
                }
                var sizeOption = sizeOptions.First();
                _wait.Until(d => sizeOption.Displayed);
                sizeOption.Click();
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine($"Stale element detected for product {i + 1}, refreshing reference.");
                if (!_driver.Url.Contains("jackets-men.html"))
                {
                    Console.WriteLine("Unexpected navigation detected, returning to product list.");
                    _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
                    _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                productItem = _driver.FindElement(ProductItem(i));
                var sizeOptions = productItem.FindElements(SizeOption);
                if (!sizeOptions.Any())
                {
                    Console.WriteLine($"No size options for product {i + 1}, skipping.");
                    continue;
                }
                var sizeOption = sizeOptions.First();
                _wait.Until(d => sizeOption.Displayed);
                sizeOption.Click();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Size option not found for product {i + 1}: {ex.Message}");
                continue;
            }

            // Chọn màu
            try
            {
                var colorOption = productItem.FindElement(ColorOption);
                _wait.Until(d => colorOption.Displayed);
                colorOption.Click();
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine($"Stale element detected for color option in product {i + 1}, refreshing reference.");
                if (!_driver.Url.Contains("jackets-men.html"))
                {
                    Console.WriteLine("Unexpected navigation detected, returning to product list.");
                    _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
                    _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                productItem = _driver.FindElement(ProductItem(i));
                var colorOption = productItem.FindElement(ColorOption);
                _wait.Until(d => colorOption.Displayed);
                colorOption.Click();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Color option not found for product {i + 1}: {ex.Message}");
                continue;
            }

            // Nhấp nút Add to Cart với hover
            try
            {
                actions.MoveToElement(productItem).Click(addToCartButton).Build().Perform();
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine($"Stale element detected for add to cart button in product {i + 1}, refreshing reference.");
                if (!_driver.Url.Contains("jackets-men.html"))
                {
                    Console.WriteLine("Unexpected navigation detected, returning to product list.");
                    _driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html");
                    _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                productItem = _driver.FindElement(ProductItem(i));
                addToCartButton = productItem.FindElement(AddToCartButton);
                actions.MoveToElement(productItem).Click(addToCartButton).Build().Perform();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Click failed for product {i + 1}: {ex.Message}");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", addToCartButton);
            }

            // Đóng mini-cart nếu hiển thị
            var miniCart = _driver.FindElements(By.CssSelector(".minicart-wrapper.active"));
            if (miniCart.Any() && miniCart.First().Displayed)
            {
                _driver.FindElement(By.CssSelector(".action.close")).Click();
                _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".minicart-wrapper.active")));
            }

            // Đợi thông báo thành công
            try
            {
                _wait.Until(d => d.FindElement(By.CssSelector(".message-success")).Displayed);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Success message not found for product {i + 1}, checking cart state.");
            }

            // Đợi trang ổn định trước khi tiếp tục
            _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}