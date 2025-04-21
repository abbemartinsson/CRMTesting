namespace ShopTester.Tests;

using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

[TestClass]
public class LoginTest : PageTest
{
  private IPlaywright _playwright;
  private IBrowser _browser;
  private IBrowserContext _browserContext;
  private IPage _page;

  private const string BaseUrl = "http://localhost:5173/";
  private const string Email = "m@email.com";
  private const string Password = "abc123";

  [TestInitialize]
  public async Task Setup()
  {
    _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,
      SlowMo = 1000
    });

    _browserContext = await _browser.NewContextAsync();
    _page = await _browserContext.NewPageAsync();
  }

  [TestCleanup]
  public async Task Cleanup()
  {
    await _browserContext.CloseAsync();
    await _browser.CloseAsync();
    _playwright.Dispose();
  }

  private async Task PerformLoginAsync()
  {
    await _page.GotoAsync(BaseUrl);
    await _page.GetByText("Login").ClickAsync();
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(Email);
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(Password);
    await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
    await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Logout" })).ToBeVisibleAsync();
  }

  [TestMethod]
  public async Task Login()
  {
    await PerformLoginAsync();
  }

  [TestMethod]
  public async Task LoginAndLookAtIssues()
  {
    await PerformLoginAsync();
    await _page.GetByText("Issues").ClickAsync();
  }

  [TestMethod]
  public async Task LoginAndSeeEmployees()
  {
    await PerformLoginAsync();
    await _page.GetByText("Employees").ClickAsync();
  }

  [TestMethod]
  public async Task ChangeIssueToOpenAndBackToNew()
  {
    await PerformLoginAsync();
    await _page.GetByText("Issues").ClickAsync();

    // Change state to OPEN
    await _page.Locator("button.subjectEditButton").First.ClickAsync();
    await _page.Locator("select.stateSelect").First.SelectOptionAsync("OPEN");
    await _page.Locator("button.stateUpdateButton").First.ClickAsync();

    // Change state back to NEW
    await _page.Locator("button.subjectEditButton").First.ClickAsync();
    await _page.Locator("select.stateSelect").First.SelectOptionAsync("NEW");
    await _page.Locator("button.stateUpdateButton").First.ClickAsync();

    await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
  }
}
