// NuGet Packet Requirements:
// [+] Selenium.WebDriver by Selenium Committers
// [+] DotNetSeleniumExtras.WaitHelpers by SeleniumExtras.WaitHelpers

using System;
using System.Collections.Generic;
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

    public void Init(bool headless = false, double scaleFactor = 1.0, string userAgentOverride = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36")
    {
        KillDriver();

        // Create Options
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
        chromeOptions.AddArgument("--log-level=3");
        chromeOptions.AddArgument("--disable-notifications");
        chromeOptions.AddArgument("--ignore-ssl-errors=yes");
        chromeOptions.AddArgument("--ignore-certificate-errors");
        chromeOptions.AddArgument("--disable-blink-features");
        chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
        chromeOptions.AddArgument($"--force-device-scale-factor={scaleFactor}");

        chromeOptions.AddExcludedArguments(new List<string>() { "enable-automation" });
        
        if(headless) 
            chromeOptions.AddArgument("--headless");

        if(!string.IsNullOrEmpty(userAgentOverride))
            chromeOptions.AddArgument($"--user-agent={userAgentOverride}");

        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;

        // Add desired capabilities
        chromeOptions.PageLoadStrategy = PageLoadStrategy.None;

        driver = new ChromeDriver(service, chromeOptions);
        driver.Manage().Window.Maximize();
        //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
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
        if(driver != null) return true;
        else return false;
    }

    // Helper Functions
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

    public int GetCurrentTimestamp()
    {
        return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public bool GetIsStringExistInElement(string selector, string str, bool byCssSelector = true)
    {
        try
        {
            IWebElement elem = driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector));

            if(elem.Text.Contains(str)) return true;
        }
        catch (Exception) { }

        return false;
    }

    public void GoTo(string url, bool waitURL = false, int timeout = 999999999)
    {
        CheckDriver();

        driver.Navigate().GoToUrl(url);
        if(waitURL) WaitUntilURL(url, timeout);
    }

    public IWebElement QuerySelector(string selector, bool byCssSelector = true)
    {
        return driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector));
    }

    public IReadOnlyCollection<IWebElement> QuerySelectorAll(string selector, bool byCssSelector = true)
    {
        return driver.FindElements(byCssSelector ? By.CssSelector(selector) : By.XPath(selector));
    }

    public object ExecuteJavascript(string code, params object[] args)
    {
        return driver.ExecuteScript($"var args = arguments;{code}", args);
    }

    public object ExecuteJavascriptNoException(string code, params object[] args)
    {
        try { return ExecuteJavascript(code, args); }
        catch(JavaScriptException) { };

        return null;
    }

    public void ClickViaJavascript(string selector)
    {
        ExecuteJavascriptNoException("document.querySelector(args[0]).click()", selector);
    }

    public void Sleep(int seconds)
    {
        SleepMS(seconds * 1000);
    }

    public void SleepMS(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    public WebDriverWait Wait(int timeout = 999999999)
    {
        CheckDriver();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(WebDriverException));

        return wait;
    }

    public void WaitUntilURL(string url, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.Url == url);
    }

    public void WaitUntilURLContainsString(string str, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.Url.Contains(str));
    }

    public void WaitUntilElementIsExist(string selector, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(ExpectedConditions.ElementExists(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)));
    }

    public void WaitUntilElementIsExistNot(string selector, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver =>
        {
            IWebElement element = null;
            
            try { element = driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)); }
            catch(NoSuchElementException) { }

            return element == null;
        });
    }

    public void WaitUntilElementIsClickable(string selector, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(ExpectedConditions.ElementToBeClickable(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)));
    }

    public void WaitUntilElementIsVisible(string selector, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(ExpectedConditions.ElementIsVisible(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)));
    }

    public void WaitUntilElementIsVisibleNot(string selector, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => !driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Displayed);
    }

    public void WaitUntilElementText(string selector, string text, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Text == text);
    }

    public void WaitUntilElementTextNot(string selector, string text, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Text != text);
    }

    public void WaitUntilElementContainsText(string selector, string text, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Text.Contains(text));
    }

    public void WaitUntilAttributeValue(string selector, string attribute, string value, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).GetAttribute(attribute) == value);
    }

    public void WaitUntilAttributeValueNot(string selector, string attribute, string value, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).GetAttribute(attribute) != value);
    }

    public void WaitUntilAttributeContainsValue(string selector, string attribute, string value, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElement(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).GetAttribute(attribute).Contains(value));
    }

    public void WaitUntilTotalElementListCount(string selector, int count, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElements(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Count == count);
    }

    public void WaitUntilTotalElementListCountNot(string selector, int count, bool byCssSelector = true, int timeout = 999999999)
    {
        CheckDriver();
        Wait(timeout).Until(driver => driver.FindElements(byCssSelector ? By.CssSelector(selector) : By.XPath(selector)).Count != count);
    }
}

public static class IWebElementExtensions
{
    public static string InnerHTML(this IWebElement e)
    {
        return e.GetAttribute("innerHTML");
    }

    public static string OuterHTML(this IWebElement e)
    {
        return e.GetAttribute("outerHTML");
    }

    public static IWebElement GetParent(this IWebElement e)
    {
        return e.FindElement(By.XPath(".."));
    }
}
