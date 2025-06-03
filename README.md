# Magento Store Test Automation

This project automates UI testing for the Magento test store using Selenium WebDriver and SpecFlow with C# in a BDD approach.

## Project Structure

- `Features/`: Contains SpecFlow feature files that define test scenarios in Gherkin syntax
- `Pages/`: Implementation of Page Object Model (POM) pattern
- `Steps/`: SpecFlow step definitions that implement the test steps

## Prerequisites

- .NET 8 SDK
- Chrome browser
- Visual Studio 2022 or JetBrains Rider

## Setup Instructions

1. Clone this repository
2. Open the solution file in Visual Studio or your preferred IDE
3. Restore NuGet packages:
   ```
   dotnet restore
   ```
4. Build the project:
   ```
   dotnet build
   ```

## Running Tests

Run tests from command line:
```
dotnet test
```

Or use Visual Studio's Test Explorer.

## Test Reports

Test reports and screenshots are automatically generated in:
- `Reports/`: Contains text-based test reports
- Root directory: Contains screenshot files for important test steps

## .gitignore Configuration

The project includes a comprehensive `.gitignore` file that excludes:

1. Build outputs:
   - `bin/` and `obj/` folders
   - Debug and Release folders

2. IDE-specific files:
   - `.vs/` folder (Visual Studio)
   - `.vscode/` folder (VS Code)
   - `.idea/` folder (JetBrains Rider)
   - Various IDE configuration and cache files

3. Test results:
   - `TestResults/` folder
   - NUnit result XML files

4. Test artifacts:
   - Screenshots (PNG, JPG, JPEG, BMP)
   - Report files in `Reports/` folder
   - Log files (including WebDriver logs)

5. NuGet package files:
   - `packages/` folder (except for build packages)

6. SpecFlow generated files:
   - `*.feature.cs` files (auto-generated code)

This ensures that only source code and configuration files are committed to version control, while all generated or environment-specific files are excluded.

## CI/CD Integration

This project can be integrated with GitHub Actions for continuous testing. Configuration file is pending implementation.