using System;
using System.Text;

namespace EB_Utility
{
    public static class StringUtil
    {
        public static string CreateUniqueString(bool base64=false, bool replaceBase64SpecialChars=false)
        {
            Guid g = Guid.NewGuid();

            if(base64)
            {
                string uniqueString = Convert.ToBase64String(g.ToByteArray());
                
                if(replaceBase64SpecialChars)
                {
                    uniqueString = uniqueString.Replace("=", "");
                    uniqueString = uniqueString.Replace("+", "");
                    uniqueString = uniqueString.Replace("/", "");
                }

                return uniqueString;
            }

            return g.ToString();
        }

        public static string CreateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            char c;
            for(int i=0; i < size; i++)
            {
                c = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(c);
            }

            return builder.ToString();
        }

        public static bool IsValidVariableName(string text)
        {
            if(string.IsNullOrEmpty(text))
                return false;

            if(!char.IsLetter(text[0]) && text[0] != '_')
                return false;

            for(int ix = 1; ix < text.Length; ++ix)
                if(!char.IsLetterOrDigit(text[ix]) && text[ix] != '_')
                   return false;
            
            return true;
        }
        
        public static bool IsValidURL(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp 
                    || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
