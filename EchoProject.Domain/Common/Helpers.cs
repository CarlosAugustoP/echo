using System.Text.RegularExpressions;

namespace EchoProject.Domain.Common
{
    public static class Helpers
    {
        public static string OnlyDigits(string input)
         => Regex.Replace(input, @"\D", "");
         
        public static decimal ToDecimal(this long l)
        {
            return l / 100m;
        }

        public static long ToLong(this decimal d)
        {
            return (long) (d * 100);
        }
    }

}