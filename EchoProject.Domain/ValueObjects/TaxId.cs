using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class TaxId : ValueObject
    {
        public string Value { get; }
        public bool IsCpf => Value.Length == 11;
        public bool IsCnpj => Value.Length == 14;

        public TaxId(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("TaxId cannot be empty.");

            var digits = Helpers.OnlyDigits(number);

            if (digits.Length == 11)
            {
                if (!IsValidCpf(digits))
                    throw new ArgumentException("Invalid CPF.");

                Value = digits;
            }
            else if (digits.Length == 14)
            {
                if (!IsValidCnpj(digits))
                    throw new ArgumentException("Invalid CNPJ.");

                Value = digits;
            }
            else
            {
                throw new ArgumentException("TaxId must be CPF (11) or CNPJ (14).");
            }
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        private static bool IsValidCpf(string cpf)
        {
            if (cpf.Distinct().Count() == 1)
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

        private static bool IsValidCnpj(string cnpj)
        {
            if (cnpj.Distinct().Count() == 1)
                return false;

            var numbers = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            int[] weight1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] weight2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var sum1 = 0;
            for (int i = 0; i < 12; i++)
                sum1 += numbers[i] * weight1[i];

            var remainder1 = sum1 % 11;
            var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

            if (numbers[12] != digit1)
                return false;

            var sum2 = 0;
            for (int i = 0; i < 13; i++)
                sum2 += numbers[i] * weight2[i];

            var remainder2 = sum2 % 11;
            var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

            return numbers[13] == digit2;
        }
    }
}