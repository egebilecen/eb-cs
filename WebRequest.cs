using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EB_Utility
{
    public static class WebRequest
    {
        // ipPort: 0.0.0.0:999
        // authPair: (<username>, <password>)
        public static WebProxy CreateProxy(string ipPort, (string, string)? authPair = null, bool useDefaultCredentials = false)
        {
            WebProxy webProxy = new WebProxy(ipPort, false);

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

        public static bool DownloadImage(string imageUrl, string filename, ImageFormat format)
        {    
            WebClient client = new WebClient();
            Stream    stream = client.OpenRead(imageUrl);

            Bitmap bitmap;  
            bitmap = new Bitmap(stream);

            bitmap?.Save(filename, format);

            stream.Flush();
            stream.Close();
            client.Dispose();

            return File.Exists(filename);
        }
    }
}
