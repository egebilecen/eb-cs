using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EB_Utility
{
    public static class WebRequest
    {
        // str: 0.0.0.0:9999:username:password
        public static Dictionary<string, string> ParseProxyStr(string str)
        {
            string[] strSplit = str.Split(':');

            return new Dictionary<string, string>
            {
                { "ip", strSplit.ElementAtOrDefault(0) },
                { "port", strSplit.ElementAtOrDefault(1) },
                { "username", strSplit.ElementAtOrDefault(2) },
                { "password", strSplit.ElementAtOrDefault(3) },
            };
        }
        
        // ip: 0.0.0.0
        // port: 9999
        // authPair: (<username>, <password>)
        public static WebProxy CreateProxy(string ip, string port, (string, string)? authPair = null, bool useDefaultCredentials = false)
        {
            WebProxy webProxy = new WebProxy($"{ip}:{port}", false);

            if(authPair != null
            && !string.IsNullOrEmpty(authPair?.Item1)
            && !string.IsNullOrEmpty(authPair?.Item2))
            {
                webProxy.UseDefaultCredentials = false;
                webProxy.Credentials = new NetworkCredential(userName: authPair?.Item1, password: authPair?.Item2);
            }
            else if(useDefaultCredentials)
                webProxy.UseDefaultCredentials = true;

            return webProxy;
        }

        public static HttpClient CreateHTTPClient(double connectionTimeout = 10.0, WebProxy webProxy = null, string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36")
        {
            CookieContainer cookieContainer     = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
                                       | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookieContainer,
                Proxy = webProxy
            };

            HttpClient httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(connectionTimeout)
            };

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "600");
            // httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");

            return httpClient;
        }

        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        // return: (<status code>, <nullable response string>)
        public static async Task<(int, string)> GetAsync(HttpClient httpClient, string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            
            int responseCode = (int)response.StatusCode;
            byte[] buffer = await response.Content.ReadAsByteArrayAsync();
            string responseContent = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return (responseCode, responseContent);
        }
        
        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        // return: (<status code>, <nullable response string>)
        public static async Task<(int, string)> PostAsync(HttpClient httpClient, string url, List<KeyValuePair<string, string>> paramList)
        {
            var content = new FormUrlEncodedContent(paramList);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            
            int responseCode = (int)response.StatusCode;
            byte[] buffer = await response.Content.ReadAsByteArrayAsync();
            string responseContent = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return (responseCode, responseContent);
        }
        
        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        // return: (<status code>, <nullable response string>)
        public static async Task<(int, string)> PostAsync(HttpClient httpClient, string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            
            int responseCode = (int)response.StatusCode;
            byte[] buffer = await response.Content.ReadAsByteArrayAsync();
            string responseContent = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return (responseCode, responseContent);
        }

        public static async Task<bool> DownloadFile(HttpClient httpClient, string url, string filePath)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            using(FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                await response.Content.CopyToAsync(fs);

            return File.Exists(filePath);
        }
    }
}
