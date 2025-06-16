UI Test Automation for Magento Test Store
Introduction
This project implements UI test automation for the Magento Test Store using Selenium WebDriver with C#, integrated with SpecFlow (BDD) and NUnit. The test scenarios cover login, purchasing, checkout, and order verification. The project integrates with GitHub Actions for CI/CD, storing test results as artifacts (reports and screenshots).
Requirements

.NET SDK: 8.0.x
Google Chrome: Latest version (auto-installed in CI/CD).
Chromedriver: Auto-installed via GitHub Actions.
Git: To clone the repository.
Magento Test Store Account:
Email: testuser@example.com
Password: Password123!



Setup

Clone repository:
git clone https://github.com/<your-username>/magento-test-automation.git
cd magento-test-automation


Restore dependencies:
dotnet restore


Install Google Chrome (if running locally):

On Ubuntu:sudo apt update
wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
sudo apt install -y ./google-chrome-stable_current_amd64.deb


On Windows/Mac: Download and install Chrome from official site.


Install Chromedriver (if running locally):

Download Chromedriver compatible with Chrome version from here.
Add Chromedriver to PATH or project directory.


Build project:
dotnet build



Running Tests Locally

Run all tests:
dotnet test


Test results:

HTML report: TestResults/ExtentReport.html (if using ExtentReports).
Screenshots (on test failure): *.png in project root.
Console logs: Displayed in terminal.


Headless mode (optional):

Set environment variable:export HEADLESS=true
dotnet test


Ensure test code handles HEADLESS:var options = new ChromeOptions();
if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
{
    options.AddArgument("--headless");
}
IWebDriver driver = new ChromeDriver(options);





Test Scenarios
The test automation covers the following steps on Magento Test Store (https://magento.softwaretestingboard.com):

Login with registered account (testuser@example.com, Password123!).
Purchase:
2 men's jackets (Men's Tops → Jackets).
1 men's pants (Men's Bottoms → Pants).


Proceed to checkout from the cart page.
Verify order summary: Check products (2 jackets, 1 pants) and price.
Enter valid shipping address:
Example: Street: 123 Main St, City: New York, Region: New York, Postcode: 10001, Telephone: 123-456-7890.


Select shipping method: Example: Flat Rate.
Place order.
Verify order in "My Orders" (https://magento.softwaretestingboard.com/sales/order/history/).

Framework:  

Selenium WebDriver: Interacts with the web UI.  
SpecFlow: Implements BDD, defining scenarios in Gherkin.  
NUnit: Unit testing framework.  
ExtentReports (optional): Generates detailed HTML reports.

Source Code Structure
magento-test-automation/
├── Features/
│   └── MagentoPurchase.feature       # Gherkin test scenarios
├── Pages/
│   ├── LoginPage.cs                  # Login page
│   ├── ProductPage.cs                # Product page (jackets, pants)
│   ├── CheckoutPage.cs               # Checkout page
│   ├── OrderPage.cs                  # Order history page
├── Steps/
│   └── PurchaseSteps.cs              # Scenario step implementations
├── Reports/                          # Test reports (ExtentReports)
├── TestResults/                      # Test results (NUnit XML)
├── *.png                             # Screenshots (on error)
├── MagentoTests.csproj               # .NET project file
└── .github/workflows/magento-test.yml # GitHub Actions configuration

CI/CD Integration with GitHub Actions
The GitHub Actions workflow is defined in .github/workflows/magento-test.yml:

Triggers: Runs on push/pull request to main or manually via workflow_dispatch.  
Environment: ubuntu-latest.  
Steps:  
Checkout code.  
Install .NET SDK 8.0.x.  
Restore dependencies (dotnet restore).  
Build project (dotnet build).  
Install Chrome and Chromedriver.  
Run tests (dotnet test) in headless mode (HEADLESS=true).



Note:  

Artifact upload (reports, screenshots) is disabled as per requirements.  
Test results are displayed in GitHub Actions console logs.

Potential Improvements

Test reporting: Re-enable artifact upload if needed:- name: Upload Test Results
  uses: actions/upload-artifact@v3
  if: failure()
  with:
    name: test-results
    path: |
      Reports/
      *.png
      TestResults/
    retention-days: 7


Customize selectors: If CSS selectors (e.g., .order-item) fail, update based on page HTML:private By OrderItems => By.CssSelector(".order, .items.order-items, .table-order-items tbody tr");


Increase timeout: If pages load slowly, extend WebDriverWait timeout:_wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));



Debugging Errors
If tests fail:  

Check console logs: Review logs in GitHub Actions or local terminal to identify the failing step (login, product addition, etc.).  
Inspect HTML: Use Chrome DevTools (F12) to verify selectors on pages:
Login: https://magento.softwaretestingboard.com/customer/account/login
Jackets: https://magento.softwaretestingboard.com/men/tops-men/jackets-men.html
Pants: https://magento.softwaretestingboard.com/men/bottoms-men/pants-men.html
My Orders: https://magento.softwaretestingboard.com/sales/order/history/


Screenshots: If artifacts are re-enabled, review *.png files for UI state during errors.  
Account and data: Ensure testuser@example.com is valid and the site has available products.

Contact
For support, create a GitHub Issue or email congvanvu0000@gmail.com.
