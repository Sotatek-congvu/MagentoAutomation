# Tự động hóa kiểm thử giao diện người dùng cho Magento Test Store

## Giới thiệu
Dự án này triển khai kiểm thử tự động giao diện người dùng (UI) cho **Magento Test Store** sử dụng **Selenium WebDriver** với **C#**, tích hợp framework **SpecFlow** (BDD) và **NUnit**. Kịch bản kiểm thử thực hiện các bước đăng nhập, mua hàng, thanh toán, và xác minh đơn hàng theo yêu cầu. Dự án được tích hợp với **GitHub Actions** để chạy CI/CD và lưu trữ kết quả kiểm thử dưới dạng artifact (báo cáo và ảnh chụp màn hình).

## Yêu cầu
- **.NET SDK**: 8.0.x
- **Google Chrome**: Phiên bản mới nhất (được cài tự động trong CI/CD).
- **Chromedriver**: Tự động cài đặt qua GitHub Actions.
- **Git**: Để clone repository.
- **Tài khoản Magento Test Store**: 
  - Email: `testuser@example.com`
  - Mật khẩu: `Password123!`

## Cài đặt

1. **Clone repository**:
   ```bash
   git clone https://github.com/<your-username>/magento-test-automation.git
   cd magento-test-automation
   ```

2. **Khôi phục dependencies**:
   ```bash
   dotnet restore
   ```

3. **Cài đặt Google Chrome** (nếu chạy cục bộ):
   - Trên **Ubuntu**:
     ```bash
     sudo apt update
     wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
     sudo apt install -y ./google-chrome-stable_current_amd64.deb
     ```
   - Trên **Windows/Mac**: Tải và cài đặt Chrome từ [trang chính thức](https://www.google.com/chrome/).

4. **Cài đặt Chromedriver** (nếu chạy cục bộ):
   - Tải Chromedriver tương thích với phiên bản Chrome từ [đây](https://chromedriver.chromium.org/downloads).
   - Thêm Chromedriver vào PATH hoặc thư mục dự án.

5. **Build dự án**:
   ```bash
   dotnet build
   ```

## Chạy kiểm thử cục bộ

1. **Chạy tất cả kiểm thử**:
   ```bash
   dotnet test
   ```

2. **Kết quả kiểm thử**:
   - Báo cáo HTML: `TestResults/ExtentReport.html` (nếu sử dụng ExtentReports).
   - Ảnh chụp màn hình (nếu kiểm thử thất bại): `*.png` trong thư mục gốc dự án.
   - Log console: Hiển thị trong terminal.

3. **Chế độ headless** (tùy chọn):
   - Thiết lập biến môi trường:
     ```bash
     export HEADLESS=true
     dotnet test
     ```
   - Đảm bảo mã kiểm thử xử lý biến `HEADLESS`:
     ```csharp
     var options = new ChromeOptions();
     if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
     {
         options.AddArgument("--headless");
     }
     IWebDriver driver = new ChromeDriver(options);
     ```

## Kịch bản kiểm thử

Kịch bản kiểm thử tự động hóa các bước sau trên **Magento Test Store** (`https://magento.softwaretestingboard.com`):

1. **Đăng nhập** với tài khoản đã đăng ký (`testuser@example.com`, `Password123!`).
2. **Mua hàng**:
   - 2 áo khoác nam (Men's Tops → Jackets).
   - 1 quần nam (Men's Bottoms → Pants).
3. **Tiến hành thanh toán** từ trang giỏ hàng.
4. **Xác minh tóm tắt đơn hàng**: Kiểm tra sản phẩm (2 jackets, 1 pants) và giá.
5. **Nhập địa chỉ giao hàng hợp lệ**:
   - Ví dụ: Street: `123 Main St`, City: `New York`, Region: `New York`, Postcode: `10001`, Telephone: `123-456-7890`.
6. **Chọn phương thức giao hàng**: Ví dụ: Flat Rate.
7. **Đặt hàng**.
8. **Xác minh đơn hàng** trong "My Orders" (`https://magento.softwaretestingboard.com/sales/order/history/`).

**Framework**:  
- **Selenium WebDriver**: Tương tác với giao diện web.  
- **SpecFlow**: Thực hiện kiểm thử theo cách tiếp cận BDD, định nghĩa kịch bản bằng Gherkin.  
- **NUnit**: Khung kiểm thử đơn vị.  
- **ExtentReports** (tùy chọn): Tạo báo cáo HTML chi tiết.

## Cấu trúc mã nguồn

```
magento-test-automation/
├── Features/
│   └── MagentoPurchase.feature       # Kịch bản kiểm thử Gherkin
├── Pages/
│   ├── LoginPage.cs                  # Trang đăng nhập
│   ├── ProductPage.cs                # Trang sản phẩm (jackets, pants)
│   ├── CheckoutPage.cs               # Trang thanh toán
│   ├── OrderPage.cs                  # Trang lịch sử đơn hàng
├── Steps/
│   └── PurchaseSteps.cs              # Bước thực thi kịch bản
├── Reports/                          # Báo cáo kiểm thử (ExtentReports)
├── TestResults/                      # Kết quả kiểm thử (NUnit XML)
├── *.png                             # Ảnh chụp màn hình (khi lỗi)
├── MagentoTests.csproj               # Tệp dự án .NET
└── .github/workflows/magento-test.yml # Cấu hình GitHub Actions
```

## Tích hợp CI/CD với GitHub Actions

Workflow GitHub Actions được định nghĩa trong `.github/workflows/magento-test.yml`:

1. **Kích hoạt**: Chạy khi push/pull request lên nhánh `main` hoặc thủ công qua `workflow_dispatch`.  
2. **Môi trường**: `ubuntu-latest`.  
3. **Các bước**:  
   - Checkout mã nguồn.  
   - Cài đặt .NET SDK 8.0.x.  
   - Khôi phục dependencies (`dotnet restore`).  
   - Build dự án (`dotnet build`).  
   - Cài đặt Chrome và Chromedriver.  
   - Chạy kiểm thử (`dotnet test`) ở chế độ headless (`HEADLESS=true`).  

**Lưu ý**:  
- Đã bỏ bước tải lên artifact (báo cáo, ảnh chụp) theo yêu cầu.  
- Kết quả kiểm thử hiển thị trong log console của GitHub Actions.

## Cải tiến tiềm năng
- **Báo cáo kiểm thử**: Nếu cần, có thể bật lại bước tải artifact:
  ```yaml
  - name: Upload Test Results
    uses: actions/upload-artifact@v3
    if: failure()
    with:
      name: test-results
      path: |
        Reports/
        *.png
        TestResults/
      retention-days: 7
  ```
- **Tùy chỉnh bộ chọn**: Nếu bộ chọn CSS (như `.order-item`) không đúng, cập nhật dựa trên HTML trang:
  ```csharp
  private By OrderItems => By.CssSelector(".order, .items.order-items, .table-order-items tbody tr");
  ```
- **Tăng thời gian chờ**: Nếu trang tải chậm, tăng timeout trong `WebDriverWait`:
  ```csharp
  _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
  ```

## Debug lỗi

Nếu kiểm thử thất bại:  
1. **Kiểm tra log console**: Xem log trong GitHub Actions hoặc terminal cục bộ để xác định bước lỗi (đăng nhập, thêm sản phẩm, v.v.).  
2. **Kiểm tra HTML**: Sử dụng Chrome DevTools (F12) để kiểm tra bộ chọn trên các trang:
   - Đăng nhập: `https://magento.softwaretestingboard.com/customer/account/login`
   - Jackets: `https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html`
   - Pants: `https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html`
   - My Orders: `https://magento.softwaretestingboard.com/sales/order/history/`
3. **Ảnh chụp màn hình**: Nếu bật lại artifact, xem ảnh chụp (`*.png`) để phân tích giao diện khi lỗi.  
4. **Tài khoản và dữ liệu**: Đảm bảo tài khoản `testuser@example.com` hợp lệ và trang có sản phẩm.

## Liên hệ
Nếu cần hỗ trợ, vui lòng liên hệ qua GitHub Issues hoặc email của bạn.

---  
**Lưu ý**: Đảm bảo tệp `README.md` được đặt trong thư mục gốc của repository. Nếu bạn cần thêm nội dung (như cách tạo tài khoản Magento, cấu hình ExtentReports), hãy yêu cầu cụ thể!