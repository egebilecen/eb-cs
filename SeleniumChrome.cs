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
    public static class JavascriptFunctions
    {
        public static string IsOverflown()
        {
            return @"
                function IsOverflown(elem)
                {
                    return elem.scrollHeight > elem.clientHeight || elem.scrollWidth > elem.clientWidth;
                }
            ";
        }

        public static string IsVisible(bool checkIfFullyVisible = false)
        {
            return @"
                function IsVisible(elem)
                {
                    let rect = elem.getBoundingClientRect();
                    let viewHeight = Math.max(document.documentElement.clientHeight, window.innerHeight);
                    return !(rect.bottom < 0 || rect.top " + (checkIfFullyVisible ? "+ rect.height" : "") + @" - viewHeight >= 0);
                }
            ";
        }
    }

    public ChromeDriver driver = null;
    public bool isHeadless { get; private set; } = false;

    private void KillDriver()
    {
        foreach(var process in Process.GetProcessesByName("chromedriver"))
            process.Kill();
    }

    private void CheckDriver()
    {
        if(driver == null) throw new WebDriverException("Error: Driver is not initialized.");
    }

    public void Init(bool headless = false, bool maximizeWindow = true, int pageLoadTimeout = 0, double scaleFactor = 1.0, (int, int)? windowSize = null, string userAgentOverride = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36", List<string> extraArgs = null)
    {
        KillDriver();
        isHeadless = headless;

        // Create Options
        var chromeOptions = new ChromeOptions();

        //chromeOptions.AddArgument("--log-level=3");
        chromeOptions.AddArgument("--disable-notifications");
        chromeOptions.AddArgument("--ignore-ssl-errors=yes");
        chromeOptions.AddArgument("--ignore-certificate-errors");
        chromeOptions.AddArgument("--disable-blink-features");
        chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
        chromeOptions.AddArgument($"--force-device-scale-factor={scaleFactor}");
        
        if(headless) 
            chromeOptions.AddArgument("--headless");

        if(windowSize != null)
            chromeOptions.AddArgument($"--window-size={windowSize?.Item1},{windowSize?.Item2}");
        
        if(!string.IsNullOrEmpty(userAgentOverride))
            chromeOptions.AddArgument($"--user-agent={userAgentOverride}");
        
        if(extraArgs != null)
            foreach(string extraArg in extraArgs)
                chromeOptions.AddArgument(extraArg);

        chromeOptions.PageLoadStrategy = PageLoadStrategy.None;
        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
        chromeOptions.AddExcludedArguments(new List<string>() { "enable-automation" });

        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;

        driver = new ChromeDriver(service, chromeOptions);

        if(maximizeWindow) 
            driver.Manage().Window.Maximize();

        if(pageLoadTimeout > 0)
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeout);
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
        }
    }

    public bool IsBrowserOpen()
    {
        if(driver != null) return true;
        else return false;
    }

    // Helper Functions
    public void SetBrowserZoomPercentage(double zoomPercentage)
    {
        if(isHeadless) return;

        const string jsCode = "let selectBox = document.querySelector(\"settings-ui\").shadowRoot.querySelector(\"#main\").shadowRoot.querySelector(\"settings-basic-page\").shadowRoot.querySelector(\"settings-appearance-page\").shadowRoot.querySelector(\"#zoomLevel\");let changeEvent = new Event(\"change\");selectBox.value = arguments[0];selectBox.dispatchEvent(changeEvent);";
        
        GoTo("chrome://settings/");
        WaitUntilElementIsExist("settings-ui", timeout: 5);

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
        return driver.ExecuteScript($"const args = arguments;{code}", args);
    }

    public object ExecuteJavascriptNoException(string code, params object[] args)
    {
        try { return ExecuteJavascript(code, args); }
        catch(JavaScriptException) { };

        return null;
    }

    public void ClickJavascript(string selector)
    {
        ExecuteJavascriptNoException("document.querySelector(args[0]).click();", selector);
    }

    public void SendKeysJavascript(string selector, string text)
    {
        ExecuteJavascriptNoException("let elem = document.querySelector(args[0]); elem.value = args[1]; elem.dispatchEvent(new Event('change'));", selector, text);
    }

    public void TriggerEvent(IWebElement elem, string eventName)
    {
        ExecuteJavascriptNoException("args[0].dispatchEvent(new Event(args[1]));", elem, eventName);
    }

    public void TriggerEvent(string selector, string eventName)
    {
        TriggerEvent(QuerySelector(selector), eventName);
    }

    public bool IsOverflown(IWebElement elem)
    {
        return (bool)ExecuteJavascriptNoException($@"
            {JavascriptFunctions.IsOverflown()}
            return IsOverflown(args[0]);
        ", elem);
    }

    public bool IsOverflown(string selector)
    {
        return IsOverflown(QuerySelector(selector));
    }

    public bool IsVisible(IWebElement elem, bool checkIfFullyVisible = false)
    {
        return (bool)ExecuteJavascriptNoException($@"
            {JavascriptFunctions.IsVisible(checkIfFullyVisible)}
            return IsVisible(args[0]);
        ", elem);
    }

    public bool IsVisible(string selector, bool checkIfFullyVisible = false)
    {
        return IsVisible(QuerySelector(selector), checkIfFullyVisible);
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
