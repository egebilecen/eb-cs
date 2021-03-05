// NuGet Packet Requirements:
// [+] System.Drawing.Common by Microsoft

using System;
using System.Drawing;
using System.IO;

namespace EB_Utility
{
    public static class Image
    {
        public static Bitmap base64_image_to_bitmap(string base64_image)
        {
            byte[]       bitmap_data   = Convert.FromBase64String(base64_image);
            MemoryStream stream_bitmap = new MemoryStream(bitmap_data);
            Bitmap       bitmap        = new Bitmap((Bitmap)System.Drawing.Image.FromStream(stream_bitmap));

            return bitmap;
        }
    }
}
