using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class ProductPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private readonly int _standardWait = 1000;
    private readonly int _longWait = 2000;

    private readonly Dictionary<string, string> _productUrls = new Dictionary<string, string> {
        { "jackets", "https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html" },
        { "pants", "https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html" }
    };

    private By ProductItem(int index) => By.CssSelector($".product-item:nth-child({index + 1})");
    private By AllProducts => By.CssSelector(".product-item");
    private By SizeOption => By.CssSelector(".swatch-attribute.size .swatch-option:first-child");
    private By ColorOption => By.CssSelector(".swatch-attribute.color .swatch-option:first-child");
    private By AddToCartButton => By.CssSelector(".action.tocart.primary");
    private By FilterToggle => By.CssSelector(".filter-options-title");
    private By FilterContent => By.CssSelector("div.filter-options-content");
    private By MiniCart => By.CssSelector(".minicart-wrapper.active");
    private By CloseButton => By.CssSelector(".action.close");
    private By SuccessMessage => By.CssSelector(".message-success");
    private By PopupCloseButton => By.CssSelector(".modal-popup .action-close, .action-dismiss, .modals-overlay");
    private By CartCounter => By.CssSelector(".counter.qty");

    public ProductPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// </summary>
    /// <param name="quantity">Số lượng áo khoác cần thêm</param>
    public void AddJacketsToCart(int quantity)
    {
        AddProductToCart("jackets", quantity);
    }

    
    public void AddPantsToCart(int quantity)
    {
        AddProductToCart("pants", quantity);
    }

    
    private void AddProductToCart(string productType, int quantity)
    {
        Console.WriteLine($"Starting Add{char.ToUpper(productType[0])}{productType.Substring(1)}ToCart method...");

        CloseAllOverlays();

        if (!_productUrls.TryGetValue(productType.ToLower(), out string url))
        {
            throw new ArgumentException($"Không hỗ trợ loại sản phẩm: {productType}");
        }

        NavigateToProductPage(url);

        LogPageState();

        var allProducts = FindAllProducts();

        AddProductsToCart(allProducts, quantity, productType);

        Console.WriteLine($"Add{char.ToUpper(productType[0])}{productType.Substring(1)}ToCart method completed");
    }

    
    private void NavigateToProductPage(string url)
    {
        _driver.Navigate().GoToUrl(url);
        WaitForPageLoad();
        Thread.Sleep(_longWait); 

        
        CloseAllOverlays();
    }

   
    private IList<IWebElement> FindAllProducts()
    {
        var products = _driver.FindElements(AllProducts);
        Console.WriteLine($"Found {products.Count} products on page");

        if (products.Count == 0)
        {
            Console.WriteLine("No products found, refreshing page");
            _driver.Navigate().Refresh();
            WaitForPageLoad();
            Thread.Sleep(_longWait);
            products = _driver.FindElements(AllProducts);
            Console.WriteLine($"After refresh: Found {products.Count} products");

            if (products.Count == 0)
            {
                TakeScreenshot("no_products_found");
                throw new NoSuchElementException("Không tìm thấy sản phẩm nào trên trang sau khi tải lại.");
            }
        }

        return products;
    }

    
    private void AddProductsToCart(IList<IWebElement> products, int quantity, string productType)
    {
        string productUrl = _productUrls[productType];
        int successCount = 0;

        for (int i = 0; i < quantity && i < products.Count; i++)
        {
            Console.WriteLine($"Processing product {i + 1}");
            bool added = false;

            try
            {
                int beforeCount = GetCartCount();

                var productItem = _driver.FindElement(ProductItem(i));

                ScrollToElement(productItem);
                CloseAllOverlays();

                CloseFilterPanel();

                HoverOverProduct(productItem);

                if (!SelectProductOptions(productItem))
                {
                    Console.WriteLine($"Skipping product {i + 1} due to inability to select options");
                    continue;
                }

                if (!ClickAddToCartButton(productItem))
                {
                    Console.WriteLine($"Skipping product {i + 1} due to inability to click Add to Cart");
                    continue;
                }
                added = VerifyProductAdded(beforeCount);

                CloseMiniCart();

                WaitForPageLoad();
                Thread.Sleep(_standardWait);

                if (added)
                {
                    successCount++;
                }
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine($"Stale element detected while processing product {i + 1}, recreating reference.");

                if (!IsOnProductPage(productUrl))
                {
                    Console.WriteLine("Unexpected navigation detected, returning to product page.");
                    NavigateToProductPage(productUrl);
                }

                i--;
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing product {i + 1}: {ex.Message}");
                TakeScreenshot($"error_product_{i + 1}");
            }
        }

        Console.WriteLine($"Successfully added {successCount} out of {quantity} requested {productType} to cart");
    }

   
    private void CloseAllOverlays()
    {
        try
        {
            var popups = _driver.FindElements(PopupCloseButton);
            foreach (var popup in popups)
            {
                try
                {
                    if (popup.Displayed && popup.Enabled)
                    {
                        popup.Click();
                        Thread.Sleep(_standardWait / 2);
                        Console.WriteLine("Closed popup");
                    }
                }
                catch (Exception) { }
            }

            CloseMiniCart();

            var successMessages = _driver.FindElements(SuccessMessage);
            foreach (var message in successMessages)
            {
                try
                {
                    if (message.Displayed)
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].style.display = 'none';", message);
                    }
                }
                catch (Exception) { }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing overlays: {ex.Message}");
        }
    }

    
    private void CloseMiniCart()
    {
        try
        {
            var miniCarts = _driver.FindElements(MiniCart);
            if (miniCarts.Any())
            {
                var miniCart = miniCarts.First();
                if (miniCart.Displayed)
                {
                    var closeButtons = _driver.FindElements(CloseButton);
                    foreach (var closeButton in closeButtons)
                    {
                        if (closeButton.Displayed && closeButton.Enabled)
                        {
                            try
                            {
                                closeButton.Click();
                                _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(MiniCart));
                                Console.WriteLine("Closed mini cart");
                                break;
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing mini cart: {ex.Message}");
        }
    }

   
    private void CloseFilterPanel()
    {
        try
        {
            var filterToggles = _driver.FindElements(FilterToggle);
            if (filterToggles.Any() && filterToggles.First().Displayed)
            {
                filterToggles.First().Click();
                try
                {
                    _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(FilterContent));
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("Filter panel did not collapse, proceeding anyway.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing filter panel: {ex.Message}");
        }
    }

   
    private void ScrollToElement(IWebElement element)
    {
        try
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', behavior: 'smooth'});", element);
            Thread.Sleep(_standardWait);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scrolling to element: {ex.Message}");
        }
    }

    
    private void HoverOverProduct(IWebElement productItem)
    {
        try
        {
            new Actions(_driver).MoveToElement(productItem).Perform();
            Thread.Sleep(_standardWait);

            ((IJavaScriptExecutor)_driver).ExecuteScript(
                "var event = new MouseEvent('mouseover', {" +
                "  bubbles: true," +
                "  cancelable: true" +
                "});" +
                "arguments[0].dispatchEvent(event);", productItem);
            Thread.Sleep(_standardWait);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error hovering over product: {ex.Message}");
        }
    }

    
    private bool SelectProductOptions(IWebElement productItem)
    {
        // Chọn kích thước
        try
        {
            var sizeOptions = productItem.FindElements(SizeOption);
            if (!sizeOptions.Any())
            {
                Console.WriteLine("No size options found");
                return false;
            }

            var sizeOption = sizeOptions.First();
            ScrollToElement(sizeOption);

            // Thử cả hai cách click thông thường và JavaScript
            try
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(sizeOption));
                sizeOption.Click();
            }
            catch (Exception)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", sizeOption);
            }

            Console.WriteLine("Size option selected");
            Thread.Sleep(_standardWait);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting size: {ex.Message}");
            return false;
        }

        // Chọn màu sắc
        try
        {
            var colorOptions = productItem.FindElements(ColorOption);
            if (!colorOptions.Any())
            {
                Console.WriteLine("No color options found");
                return false;
            }

            var colorOption = colorOptions.First();
            ScrollToElement(colorOption);

            // Thử cả hai cách click thông thường và JavaScript
            try
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(colorOption));
                colorOption.Click();
            }
            catch (Exception)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", colorOption);
            }

            Console.WriteLine("Color option selected");
            Thread.Sleep(_standardWait);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting color: {ex.Message}");
            return false;
        }

        return true;
    }

    private bool ClickAddToCartButton(IWebElement productItem)
    {
        try
        {
            // Tìm lại nút "Add to Cart" sau khi chọn tùy chọn
            IWebElement addToCartButton;
            try
            {
                addToCartButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    productItem.FindElement(AddToCartButton)));
            }
            catch (Exception)
            {
                // Nếu không tìm thấy nút, thử làm hiển thị nó bằng JavaScript
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "if (arguments[0].querySelector('.actions-primary')) { " +
                    "  arguments[0].querySelector('.actions-primary').style.visibility = 'visible'; " +
                    "  arguments[0].querySelector('.actions-primary').style.display = 'block'; " +
                    "}",
                    productItem);
                Thread.Sleep(_standardWait);

                addToCartButton = productItem.FindElement(AddToCartButton);
            }

            // Cuộn đến nút
            ScrollToElement(addToCartButton);

            // Thử ba phương pháp để click nút

            // Phương pháp 1: Click thông thường
            try
            {
                addToCartButton.Click();
                Console.WriteLine("Clicked Add to Cart button normally");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Normal click failed: {ex.Message}");
            }

            // Phương pháp 2: Sử dụng Actions API
            try
            {
                new Actions(_driver)
                    .MoveToElement(addToCartButton)
                    .Click()
                    .Build()
                    .Perform();
                Console.WriteLine("Clicked Add to Cart button using Actions API");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Actions click failed: {ex.Message}");
            }

            // Phương pháp 3: Sử dụng JavaScript
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", addToCartButton);
                Console.WriteLine("Clicked Add to Cart button using JavaScript");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JavaScript click failed: {ex.Message}");
            }

            // Nếu tất cả đều thất bại
            Console.WriteLine("All click methods failed");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clicking Add to Cart button: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Xác minh sản phẩm đã được thêm vào giỏ hàng
    /// </summary>
    /// <param name="beforeCount">Số lượng sản phẩm trong giỏ trước khi thêm</param>
    /// <returns>true nếu thành công, false nếu thất bại</returns>
    private bool VerifyProductAdded(int beforeCount)
    {
        try
        {
            // Phương pháp 1: Kiểm tra thông báo thành công
            try
            {
                _wait.Until(d => d.FindElement(SuccessMessage).Displayed);
                Console.WriteLine("Success message displayed");
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Success message not found, checking cart count");
            }

            // Phương pháp 2: Kiểm tra số lượng trong giỏ hàng
            int afterCount = GetCartCount();
            if (afterCount > beforeCount)
            {
                Console.WriteLine($"Cart count increased from {beforeCount} to {afterCount}");
                return true;
            }

            Console.WriteLine($"Cart count did not increase: before={beforeCount}, after={afterCount}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying product added: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Lấy số lượng sản phẩm hiện tại trong giỏ hàng
    /// </summary>
    /// <returns>Số lượng sản phẩm trong giỏ hàng</returns>
    private int GetCartCount()
    {
        try
        {
            var counterElements = _driver.FindElements(CartCounter);
            if (counterElements.Any())
            {
                var counterText = counterElements.First().Text;
                if (int.TryParse(counterText, out int count))
                {
                    return count;
                }

                // Nếu không phải số, cố gắng trích xuất số từ chuỗi
                var digits = new string(counterText.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out count))
                {
                    return count;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cart count: {ex.Message}");
        }

        return 0;
    }

    /// <summary>
    /// Kiểm tra xem có đang ở trang sản phẩm không
    /// </summary>
    /// <param name="expectedUrl">URL trang sản phẩm mong đợi</param>
    /// <returns>true nếu đang ở trang sản phẩm, false nếu không</returns>
    private bool IsOnProductPage(string expectedUrl)
    {
        string currentUrl = _driver.Url;
        bool isOnProductPage = currentUrl.Contains(new Uri(expectedUrl).PathAndQuery);
        return isOnProductPage;
    }

    /// <summary>
    /// Đợi trang tải hoàn tất
    /// </summary>
    private void WaitForPageLoad()
    {
        _wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
    }

    /// <summary>
    /// Ghi log trạng thái trang hiện tại
    /// </summary>
    private void LogPageState()
    {
        Console.WriteLine($"Current URL: {_driver.Url}");
        Console.WriteLine($"Page title: {_driver.Title}");
    }

    /// <summary>
    /// Chụp ảnh màn hình để debug
    /// </summary>
    /// <param name="filename">Tên file ảnh</param>
    private void TakeScreenshot(string filename)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            var fullFilename = $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            screenshot.SaveAsFile(fullFilename);
            Console.WriteLine($"Saved screenshot to {fullFilename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error taking screenshot: {ex.Message}");
        }
    }
    public void AddPantsToCart1(int quantity)
    {
        Console.WriteLine($"Starting AddPantsToCart method with quantity {quantity}...");

        // Điều hướng trực tiếp đến trang quần
        _driver.Navigate().GoToUrl(_productUrls["pants"]);
        WaitForPageLoad();
        Thread.Sleep(_longWait * 2);

        // Chụp ảnh để debug
        TakeScreenshot("pants_page_loaded");

        // Đóng các popup/overlay
        CloseAllOverlays();

        // Tìm các liên kết sản phẩm thay vì container sản phẩm
        var productLinks = _driver.FindElements(By.CssSelector(".product-item-link"));
        Console.WriteLine($"Found {productLinks.Count} pants product links");

        int successCount = 0;

        for (int i = 0; i < quantity && i < productLinks.Count; i++)
        {
            try
            {
                // Lưu số lượng giỏ hàng trước khi thêm
                int beforeCount = GetCartCount();

                // Sử dụng JavaScript để click vào liên kết sản phẩm để mở trang chi tiết
                var productLink = productLinks[i];
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", productLink);
                WaitForPageLoad();

                // Trên trang chi tiết sản phẩm, chọn size và color
                var sizeOptions = _driver.FindElements(By.CssSelector(".swatch-attribute.size .swatch-option"));
                if (sizeOptions.Count > 0)
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", sizeOptions[0]);
                    Thread.Sleep(_standardWait);
                }
                else
                {
                    Console.WriteLine("No size options found, skipping product");
                    _driver.Navigate().GoToUrl(_productUrls["pants"]);
                    WaitForPageLoad();
                    productLinks = _driver.FindElements(By.CssSelector(".product-item-link"));
                    continue;
                }

                var colorOptions = _driver.FindElements(By.CssSelector(".swatch-attribute.color .swatch-option"));
                if (colorOptions.Count > 0)
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", colorOptions[0]);
                    Thread.Sleep(_standardWait);
                }

                // Click nút Add to Cart
                var addToCartBtn = _driver.FindElement(By.Id("product-addtocart-button"));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", addToCartBtn);

                // Kiểm tra thành công
                try
                {
                    _wait.Until(d => d.FindElements(SuccessMessage).Count > 0);
                    successCount++;
                    Console.WriteLine("Product added successfully");
                }
                catch (WebDriverTimeoutException)
                {
                    int afterCount = GetCartCount();
                    if (afterCount > beforeCount)
                    {
                        successCount++;
                        Console.WriteLine($"Product added successfully (cart count: {beforeCount}->{afterCount})");
                    }
                }

                if (i < quantity - 1)
                {
                    _driver.Navigate().GoToUrl(_productUrls["pants"]);
                    WaitForPageLoad();
                    productLinks = _driver.FindElements(By.CssSelector(".product-item-link"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding pants product: {ex.Message}");
                TakeScreenshot($"error_adding_pants_{i + 1}");
                _driver.Navigate().GoToUrl(_productUrls["pants"]);
                WaitForPageLoad();
                productLinks = _driver.FindElements(By.CssSelector(".product-item-link"));
            }
        }

        Console.WriteLine($"Successfully added {successCount} out of {quantity} pants to cart");
    }
}