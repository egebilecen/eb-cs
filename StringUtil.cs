using System;
using System.Text;

namespace EB_Utility
{
    public static class StringUtil
    {
        public static string CreateUniqueString()
        {
            Guid g = Guid.NewGuid();

            string uniqueString = Convert.ToBase64String(g.ToByteArray());
            uniqueString = uniqueString.Replace("=","");
            uniqueString = uniqueString.Replace("+","");

            return uniqueString;
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
    }
}
