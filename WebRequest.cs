using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace EB_Utility
{
    public static class WebRequest
    {
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
