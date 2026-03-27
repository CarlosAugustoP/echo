using System.Numerics;
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

        public static Stream ToStream(this string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            return new MemoryStream(bytes);
        }

        public static BigInteger ToWei(this decimal d)
        {
           BigInteger weiPerEth = BigInteger.Pow(10, 18);
           return (BigInteger)(d * (decimal)weiPerEth);
        }

        public static bool ValidCNPJ(string cnpj)
        {
            if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
                return false;

            var numbers = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            var sum1 = 0;
            for (int i = 0; i < 12; i++)
                sum1 += numbers[i] * (i < 4 ? 5 - i : 13 - i);

            var remainder1 = sum1 % 11;
            var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

            if (numbers[12] != digit1)
                return false;

            var sum2 = 0;
            for (int i = 0; i < 13; i++)
                sum2 += numbers[i] * (i < 5 ? 6 - i : 14 - i);

            var remainder2 = sum2 % 11;
            var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

            return numbers[13] == digit2;
        }

        public static bool ValidCPF(string cpf)
        {
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            var sum1 = 0;
            for (int i = 0; i < 9; i++)
                sum1 += numbers[i] * (10 - i);

            var remainder1 = sum1 % 11;
            var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

            if (numbers[9] != digit1)
                return false;

            var sum2 = 0;
            for (int i = 0; i < 10; i++)
                sum2 += numbers[i] * (11 - i);

            var remainder2 = sum2 % 11;
            var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

            return numbers[10] == digit2;
        }

        public static bool ValidTaxId(string taxId)
        {
            var digits = OnlyDigits(taxId);
            return digits.Length == 11 ? ValidCPF(digits) : digits.Length == 14 && ValidCNPJ(digits);
        }

        public static bool IsSpecialCharacter(this char c)
        {
            return !char.IsLetterOrDigit(c);
        }


    }

}