using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EB_Utility
{
    public static class WebRequest
    {
        public static HttpClient CreateHTTPClient(double connectionTimeout=10.0, string userAgent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36")
        {
            CookieContainer cookieContainer     = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
                                       | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookieContainer
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

            if(response.IsSuccessStatusCode)
                return (responseCode, await response.Content.ReadAsStringAsync());

            return (responseCode, null);
        }
        
        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        // return: (<status code>, <nullable response string>)
        public static async Task<(int, string)> PostAsync(HttpClient httpClient, string url, List<KeyValuePair<string, string>> paramList)
        {
            var content = new FormUrlEncodedContent(paramList);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            int responseCode = (int)response.StatusCode;

            if(response.IsSuccessStatusCode)
                return (responseCode, await response.Content.ReadAsStringAsync());

            return (responseCode, null);
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
