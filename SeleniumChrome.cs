// NuGet Packet Requirements:
// [+] Selenium.WebDriver by Selenium Committers
// [+] DotNetSeleniumExtras.WaitHelpers by SeleniumExtras.WaitHelpers

using System;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class SeleniumChrome
{
    public ChromeDriver driver = null;

    private void KillDriver()
    {
        foreach(var process in Process.GetProcessesByName("chromedriver"))
            process.Kill();
    }

    public void Init(bool headless=false, string userAgentOverride="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36")
    {
        KillDriver();

        // Create Options
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
        chromeOptions.AddArgument("--log-level=3");
        chromeOptions.AddArgument("--disable-notifications");

        if(!string.IsNullOrEmpty(userAgentOverride))
            chromeOptions.AddArgument($"--user-agent={userAgentOverride}");
        //chromeOptions.AddArgument("--force-device-scale-factor=1.2");

        if(headless) chromeOptions.AddArgument("--headless");

        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;

        // Add desired capabilities
        chromeOptions.PageLoadStrategy = PageLoadStrategy.None;

        driver = new ChromeDriver(service, chromeOptions);
        driver.Manage().Window.Maximize();
        //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
    }

    // Helper Functions
    public void GoTo(string url, bool waitURL=false, int timeout=999999999)
    {
        CheckDriver();

        driver.Navigate().GoToUrl(url);
        if(waitURL) WaitUntilURL(url, timeout);
    }
    
    public void SetBrowserZoomPercentage(double zoomPercentage)
    {
        const string jsCode = "var selectBox = document.querySelector(\"settings-ui\").shadowRoot.querySelector(\"#main\").shadowRoot.querySelector(\"settings-basic-page\").shadowRoot.querySelector(\"settings-appearance-page\").shadowRoot.querySelector(\"#zoomLevel\");var changeEvent = new Event(\"change\");selectBox.value = arguments[0];selectBox.dispatchEvent(changeEvent);";
        
        GoTo("chrome://settings/");
        WaitUntilElementIsExist("settings-ui");

        driver.ExecuteScript(jsCode, new object[]{ Math.Round(zoomPercentage / 100.0, 2) });
    }

    public bool SetZoomPercentageOfElement(string selector, uint zoomPercentage)
    {
        try
        {
            driver.ExecuteScript("document.querySelector(\""+selector+"\").style.zoom=\""+zoomPercentage.ToString()+"%\"");
            return true;
        }
        catch(JavaScriptException)
        {
            return false;
        }
    }

    public void Close()
    {
        try
        {
            CheckDriver();

            driver.Quit();
            driver = null;
        }
        catch(WebDriverException)
        {
            //pass
        }
    }

    public void CheckDriver()
    {
        if(driver == null) throw new WebDriverException("Error: Driver is not initialized.");
    }

    public bool IsBrowserOpen()
    {
        if (driver != null) return true;
        else return false;
    }

    public int GetCurrentTimestamp()
    {
        return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public IWebElement GetParent(IWebElement e)
    {
       return e.FindElement(By.XPath(".."));
    }

    public bool GetIsStringExistInElement(string selector, string str)
    {
        try
        {
            IWebElement elem = driver.FindElement(By.CssSelector(selector));

            if(elem.Text.Contains(str)) return true;
        }
        catch (Exception) { }

        return false;
    }

    public void WaitUntilURL(string url, int timeout=999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(driver => driver.Url == url);
    }

    public void WaitUntilURLContainsString(string str, int timeout=999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(driver => driver.Url.Contains(str));
    }

    public void WaitUntilElementIsExist(string selector, int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(
            ExpectedConditions.ElementExists(By.CssSelector(selector))
        );
    }

    public void WaitUntilElementIsExistNot(string selector, int loop_delay_ms = 1000, int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(driver => driver.FindElement(By.CssSelector(selector)) == null);
    }

    public void WaitUntilElementIsClickable(string selector, int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(
            ExpectedConditions.ElementToBeClickable(By.CssSelector(selector))
        );
    }

    public void WaitUntilElementValue(string selector, string value, int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(driver => driver.FindElement(By.CssSelector(selector)).Text == value);
    }

    public void WaitUntilElementValueNot(string selector, string value, int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));
        wait.Until(driver => driver.FindElement(By.CssSelector(selector)).Text != value);
    }

    public void WaitUntilTotalElementListCount(string selector, int count, int loop_delay_ms = 1000, int timeout = 999999999)
    {
        CheckDriver();

        int start_time = GetCurrentTimestamp();

        while(GetCurrentTimestamp() - start_time < timeout)
        {
            var elem_list = driver.FindElements(By.CssSelector(selector));

            if(elem_list.Count == count) return;

            Thread.Sleep(loop_delay_ms);
        }

        throw new WebDriverTimeoutException("Timed out while waiting total element count for \""+selector+"\" to be "+count.ToString()+".");
    }

    public void WaitUntilTotalElementListCountNot(string selector, int count, int loop_delay_ms = 1000, int timeout = 999999999)
    {
        CheckDriver();

        int start_time = GetCurrentTimestamp();

        while(GetCurrentTimestamp() - start_time < timeout)
        {
            var elem_list = driver.FindElements(By.CssSelector(selector));

            if(elem_list.Count != count) return;

            Thread.Sleep(loop_delay_ms);
        }

        throw new WebDriverTimeoutException("Timed out while waiting total element count for \""+selector+"\" to be not "+count.ToString()+".");
    }

    public void WaitUntilStringExistInElement(string selector, string str, int loop_delay_ms = 1000, int timeout = 999999999)
    {
        CheckDriver();

        int start_time = GetCurrentTimestamp();

        while(GetCurrentTimestamp() - start_time < timeout)
        {
            if(GetIsStringExistInElement(selector, str) == true) return;

            Thread.Sleep(loop_delay_ms);
        }

        throw new WebDriverTimeoutException("Timed out while waiting for supplied string to be exist in \""+selector+"\".");
    }
}
