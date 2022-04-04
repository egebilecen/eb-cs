using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;

namespace EB_Utility
{
    public static class WebRequest
    {
        public static HttpClient CreateHTTPClient(double connectionTimeout=10.0, string userAgent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
        {
            CookieContainer cookieContainer     = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            httpClientHandler.AllowAutoRedirect = true;
            httpClientHandler.UseCookies        = true;
            httpClientHandler.CookieContainer   = cookieContainer;

            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(connectionTimeout);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "600");

            return httpClient;
        }

        public static void DownloadImage(string imageUrl, string filename, ImageFormat format)
        {    
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap;  bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}
