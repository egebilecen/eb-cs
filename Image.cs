using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EB_Utility
{
    public static class Image
    {
        private static readonly ImageConverter imageConverter = new ImageConverter();

        public static byte[] ImageToByteArray(Bitmap image, ImageFormat imageFormat = null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, imageFormat ?? ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        public static string ImageToBase64Str(Bitmap image)
        {
            return ByteArrayToBase64Str(ImageToByteArray(image));
        }

        public static string ByteArrayToBase64Str(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }

        // https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array/16576471#16576471
        public static Bitmap ByteArrayToImage(byte[] imageBytes)
        {
            Bitmap image = (Bitmap)imageConverter.ConvertFrom(imageBytes);

            if (image != null 
            && (image.HorizontalResolution != (int)image.HorizontalResolution 
                || image.VerticalResolution != (int)image.VerticalResolution))
            {
                // Correct a strange glitch that has been observed in the test program when converting 
                //  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts" 
                //  slightly away from the nominal integer value
                image.SetResolution((int)(image.HorizontalResolution + 0.5f), (int)(image.VerticalResolution + 0.5f));
            }

            return image;
        }

        public static Bitmap Base64StrToImage(string base64Str)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Str);
            return ByteArrayToImage(imageBytes);
        }

        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            foreach(ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
            {
                if(encoder.MimeType == mimeType)
                    return encoder;
            }

            throw new ArgumentOutOfRangeException(string.Format("'{0}' not supported", mimeType));
        }

        // quality: 1 - 100
        public static byte[] Compress(byte[] imageBytes, int quality, ImageCodecInfo codecInfo = null)
        {
            return ImageToByteArray(
                Compress(ByteArrayToImage(imageBytes), quality, codecInfo)
            );
        }

        // quality: 1 - 100
        public static Bitmap Compress(Bitmap image, int quality, ImageCodecInfo codecInfo = null)
        {
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            using(MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, codecInfo ?? GetCodecInfo("image/jpeg"), parameters);

                Bitmap compressedImage = ByteArrayToImage(memoryStream.ToArray());
                return compressedImage;
            }
        }
    }
}
