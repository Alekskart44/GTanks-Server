using System.Text;
using System.Text.RegularExpressions;

namespace Tanks_Sever.tanks.utils
{
    public static class StringUtils
    {
        public static string TrimChars(string src)
        {
            return Regex.Replace(src, @"(.)\1+", "$1");
        }

        public static string ConcatStrings(params string[] str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string adder in str)
            {
                sb.Append(adder);
            }
            return sb.ToString();
        }

        public static string ConcatMassive(string[] src, int start)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < src.Length; i++)
            {
                sb.Append(src[i]);
                if (i != src.Length - 1)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }
    }
}
