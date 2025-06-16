Chắc chắn rồi\! Dưới đây là phiên bản README được cấu trúc lại và trình bày đẹp mắt hơn, sử dụng các yếu tố Markdown để tăng tính rõ ràng và chuyên nghiệp.

-----

# 🚀 UI Test Automation for Magento Store

> Một dự án tự động hóa kiểm thử giao diện người dùng (UI) cho cửa hàng Magento Test Store. Dự án sử dụng Selenium WebDriver với C\#, tích hợp SpecFlow (BDD) và NUnit để kiểm thử các quy trình đăng nhập, mua hàng, thanh toán và xác minh đơn hàng. Tích hợp sẵn GitHub Actions cho CI/CD để tự động chạy kiểm thử và lưu trữ kết quả.

## 📋 Mục lục

  - [✨ Tính năng](https://www.google.com/search?q=%23-t%C3%ADnh-n%C4%83ng)
  - [🛠️ Công nghệ sử dụng](https://www.google.com/search?q=%23%EF%B8%8F-c%C3%B4ng-ngh%E1%BB%87-s%E1%BB%AD-d%E1%BB%A5ng)
  - [🚀 Bắt đầu](https://www.google.com/search?q=%23-b%E1%BA%AFt-%C4%91%E1%BA%A7u)
      - [📋 Yêu cầu](https://www.google.com/search?q=%23-y%C3%AAu-c%E1%BA%A7u)
      - [⚙️ Cài đặt](https://www.google.com/search?q=%23%EF%B8%8F-c%C3%A0i-%C4%91%E1%BA%B7t)
  - [🧪 Chạy kiểm thử](https://www.google.com/search?q=%23-ch%E1%BA%A1y-ki%E1%BB%83m-th%E1%BB%AD)
      - [Chạy nội bộ (Locally)](https://www.google.com/search?q=%23ch%E1%BA%A1y-n%E1%BB%99i-b%E1%BB%99-locally)
      - [Chế độ không giao diện (Headless Mode)](https://www.google.com/search?q=%23ch%E1%BA%BF-%C4%91%E1%BB%99-kh%C3%B4ng-giao-di%E1%BB%87n-headless-mode)
  - [🎯 Kịch bản kiểm thử](https://www.google.com/search?q=%23-k%E1%BB%8Bch-b%E1%BA%A3n-ki%E1%BB%83m-th%E1%BB%AD)
  - [📁 Cấu trúc thư mục](https://www.google.com/search?q=%23-c%E1%BA%A5u-tr%C3%BAc-th%C6%B0-m%E1%BB%A5c)
  - [🤖 Tích hợp CI/CD với GitHub Actions](https://www.google.com/search?q=%23-t%C3%ADch-h%E1%BB%A3p-cicd-v%E1%BB%9Bi-github-actions)
  - [📈 Cải tiến tiềm năng](https://www.google.com/search?q=%23-c%E1%BA%A3i-ti%E1%BA%BFn-ti%E1%BB%81m-n%C4%83ng)
  - [🐛 Gỡ lỗi](https://www.google.com/search?q=%23-g%E1%BB%A1-l%E1%BB%97i)
  - [📞 Liên hệ](https://www.google.com/search?q=%23-li%C3%AAn-h%E1%BB%87)

## ✨ Tính năng

  - **Kiểm thử End-to-End**: Bao gồm các luồng nghiệp vụ quan trọng: Đăng nhập, Thêm sản phẩm vào giỏ hàng, Thanh toán và Xác minh đơn hàng.
  - **Phát triển hướng hành vi (BDD)**: Kịch bản được viết bằng ngôn ngữ tự nhiên (Gherkin) với SpecFlow.
  - **Mô hình đối tượng trang (Page Object Model)**: Cấu trúc code rõ ràng, dễ bảo trì bằng cách tách biệt logic của trang và logic kiểm thử.
  - **Tích hợp CI/CD**: Tự động hóa việc chạy kiểm thử trên mỗi lần `push` hoặc `pull request` tới nhánh `main` bằng GitHub Actions.
  - **Báo cáo và Chụp màn hình**: Tự động tạo báo cáo kiểm thử (ExtentReports) và chụp ảnh màn hình khi có lỗi xảy ra.

## 🛠️ Công nghệ sử dụng

  - **Framework**: .NET 8.0
  - **UI Automation**: Selenium WebDriver
  - **Testing Framework**: NUnit
  - **BDD Framework**: SpecFlow
  - **Reporting**: ExtentReports (Tùy chọn)
  - **CI/CD**: GitHub Actions

## 🚀 Bắt đầu

### 📋 Yêu cầu

  - [.NET SDK](https://dotnet.microsoft.com/download): phiên bản `8.0.x`.
  - [Google Chrome](https://www.google.com/chrome/): phiên bản mới nhất.
  - [Git](https://git-scm.com/downloads).
  - **Tài khoản Magento Test Store**:
      - **Email**: `testuser@example.com`
      - **Password**: `Password123!`

### ⚙️ Cài đặt

1.  **Clone repository:**

    ```bash
    git clone https://github.com/<your-username>/magento-test-automation.git
    cd magento-test-automation
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore
    ```

3.  **Cài đặt Google Chrome (nếu chạy nội bộ):**

      - **Trên Ubuntu:**
        ```bash
        wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
        sudo apt update
        sudo apt install -y ./google-chrome-stable_current_amd64.deb
        ```
      - **Trên Windows/Mac:** Tải và cài đặt từ [trang chủ của Chrome](https://www.google.com/chrome/).

4.  **Cài đặt Chromedriver (nếu chạy nội bộ):**

      - Tải phiên bản Chromedriver tương thích với phiên bản Chrome của bạn từ [đây](https://googlechromelabs.github.io/chrome-for-testing/).
      - Thêm đường dẫn đến file `chromedriver` vào biến môi trường `PATH` hoặc đặt nó trong thư mục gốc của dự án.

    > **Lưu ý**: Trong môi trường CI/CD (GitHub Actions), Chrome và Chromedriver sẽ được tự động cài đặt.

5.  **Build dự án:**

    ```bash
    dotnet build
    ```

## 🧪 Chạy kiểm thử

### Chạy nội bộ (Locally)

  - **Chạy tất cả các test:**
    ```bash
    dotnet test
    ```
  - **Kết quả kiểm thử:**
      - **HTML Report**: `TestResults/ExtentReport.html` (nếu sử dụng ExtentReports).
      - **Screenshots (khi có lỗi)**: Các file `*.png` trong thư mục gốc của dự án.
      - **Console Logs**: Hiển thị trực tiếp trên terminal.

### Chế độ không giao diện (Headless Mode)

Bạn có thể chạy kiểm thử ở chế độ headless (không mở trình duyệt) để tăng tốc độ và phù hợp với môi trường server.

1.  **Thiết lập biến môi trường:**
    ```bash
    export HEADLESS=true
    ```
2.  **Chạy test:**
    ```bash
    dotnet test
    ```
3.  **Đảm bảo code C\# hỗ trợ headless mode:**
    ```csharp
    var options = new ChromeOptions();
    if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
    {
        options.AddArgument("--headless");
    }
    IWebDriver driver = new ChromeDriver(options);
    ```

## 🎯 Kịch bản kiểm thử

Các kịch bản kiểm thử tự động được thực hiện trên trang [Magento Test Store](https://magento.softwaretestingboard.com) theo các bước sau:

1.  **Đăng nhập**: Sử dụng tài khoản đã đăng ký (`testuser@example.com`, `Password123!`).
2.  **Mua sắm**:
      - Thêm **2** áo khoác nam (Men's Tops → Jackets).
      - Thêm **1** quần dài nam (Men's Bottoms → Pants).
3.  **Thanh toán**: Điều hướng đến trang thanh toán từ giỏ hàng.
4.  **Xác minh tóm tắt đơn hàng**: Kiểm tra sản phẩm (2 áo khoác, 1 quần) và tổng giá tiền.
5.  **Nhập địa chỉ giao hàng**:
      - *Ví dụ*: Street: `123 Main St`, City: `New York`, Region: `New York`, Postcode: `10001`, Telephone: `123-456-7890`.
6.  **Chọn phương thức vận chuyển**: Ví dụ: `Flat Rate`.
7.  **Đặt hàng**.
8.  **Xác minh đơn hàng**: Kiểm tra đơn hàng vừa đặt trong mục "[My Orders](https://magento.softwaretestingboard.com/sales/order/history/)".

## 📁 Cấu trúc thư mục

```
magento-test-automation/
├── .github/workflows/
│   └── magento-test.yml        # Cấu hình GitHub Actions CI/CD
├── Features/
│   └── MagentoPurchase.feature   # Kịch bản kiểm thử Gherkin
├── Pages/
│   ├── LoginPage.cs            # Page Object cho trang Đăng nhập
│   ├── ProductPage.cs          # Page Object cho trang Sản phẩm
│   ├── CheckoutPage.cs         # Page Object cho trang Thanh toán
│   └── OrderPage.cs            # Page Object cho trang Lịch sử đơn hàng
├── Steps/
│   └── PurchaseSteps.cs        # Implement các bước của kịch bản
├── Reports/                      # Chứa báo cáo test (ExtentReports)
├── TestResults/                  # Chứa kết quả test (NUnit XML)
├── *.png                         # Screenshots khi có lỗi
└── MagentoTests.csproj           # File dự án .NET
```

## 🤖 Tích hợp CI/CD với GitHub Actions

Workflow được định nghĩa trong file `.github/workflows/magento-test.yml`:

  - **Triggers**: Chạy tự động khi có `push` hoặc `pull_request` vào nhánh `main`, hoặc chạy thủ công (`workflow_dispatch`).
  - **Environment**: `ubuntu-latest`.
  - **Các bước chính**:
    1.  Checkout code.
    2.  Cài đặt .NET SDK `8.0.x`.
    3.  Restore dependencies (`dotnet restore`).
    4.  Build dự án (`dotnet build`).
    5.  Cài đặt Chrome và Chromedriver.
    6.  Chạy test ở chế độ headless (`dotnet test` với `HEADLESS=true`).

> **Lưu ý**: Việc tải lên các artifacts (báo cáo, screenshots) hiện đang bị vô hiệu hóa. Kết quả kiểm thử sẽ được hiển thị trong logs của GitHub Actions.

## 📈 Cải tiến tiềm năng

  - **Kích hoạt lại việc tải lên báo cáo**:
    Để tự động lưu trữ báo cáo và ảnh chụp màn hình khi test thất bại, hãy bỏ ghi chú đoạn code sau trong file `magento-test.yml`:

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

  - **Tùy chỉnh Selectors**: Nếu các CSS selectors (ví dụ: `.order-item`) không còn chính xác, hãy cập nhật chúng dựa trên mã HTML mới của trang.

    ```csharp
    private By OrderItems => By.CssSelector(".order, .items.order-items, .table-order-items tbody tr");
    ```

  - **Tăng thời gian chờ (Timeout)**: Nếu trang web tải chậm, hãy tăng thời gian chờ của `WebDriverWait`.

    ```csharp
    _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    ```

## 🐛 Gỡ lỗi

Nếu kiểm thử thất bại, hãy làm theo các bước sau:

1.  **Kiểm tra Console Logs**: Xem lại logs trong terminal hoặc trên GitHub Actions để xác định bước bị lỗi (đăng nhập, thêm sản phẩm, v.v.).
2.  **Kiểm tra HTML**: Sử dụng Chrome DevTools (F12) để xác minh các selectors trên các trang tương ứng:
      - **Login**: `https://magento.softwaretestingboard.com/customer/account/login`
      - **Jackets**: `https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html`
      - **Pants**: `https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html`
      - **My Orders**: `https://magento.softwaretestingboard.com/sales/order/history/`
3.  **Xem Screenshots**: Nếu đã kích hoạt tính năng tải lên artifact, hãy xem các file `*.png` để thấy trạng thái giao diện tại thời điểm xảy ra lỗi.
4.  **Kiểm tra tài khoản và dữ liệu**: Đảm bảo tài khoản `testuser@example.com` hợp lệ và các sản phẩm cần mua vẫn còn trên trang web.

## 📞 Liên hệ

Để được hỗ trợ, vui lòng tạo một [GitHub Issue](https://www.google.com/search?q=https://github.com/%3Cyour-username%3E/magento-test-automation/issues) hoặc gửi email đến congvanvu0000@gmail.com.