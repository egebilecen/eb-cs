using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace SeleniumBotProgram
{
    public static class SeleniumChromeExtensions
    {
        public static void WaitUntilElementIsExistXPath(this SeleniumChrome bot, string selector, int timeout = 999999999)
        {
            bot.CheckDriver();
            var wait = new WebDriverWait(bot.driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(WebDriverException));
            wait.Until(
                ExpectedConditions.ElementExists(By.XPath(selector))
            );
        }
    }
}
