using System;
using System.Drawing;
using System.IO;

namespace EB_Utility
{
    public static class Image
    {
        public static Bitmap Base64ImageToBitmap(string base64Image)
        {
            byte[] bitmapData = Convert.FromBase64String(base64Image);
            MemoryStream bitmapMemoryStream = new MemoryStream(bitmapData);
            Bitmap bitmap = new Bitmap((Bitmap)System.Drawing.Image.FromStream(bitmapMemoryStream));

            return bitmap;
        }
    }
}
