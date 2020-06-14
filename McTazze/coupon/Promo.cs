/*
    Copyright 2020 ErikPelli

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Globalization;

namespace McTazze.coupon
{
    // Class that generate a coupon as string from its informations
    class Promo
    {
        // Private class variables
        private readonly static int daysDiff;
        private readonly static string dictionary = "ABCDEFGHIJKLMNOPRSTUWXZ45679";
        private readonly static string startDay = "2014-12-31";

        // Promo constructor
        static Promo()
        {
            // Yesterday date (the bot uses coupon of the day before)
            DateTime Yesterday = DateTime.Today.AddDays(-1);

            DateTime StartDate = DateTime.ParseExact(startDay, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Get number of days between 31th december 2014 (a constant) and yesterday
            daysDiff = (Yesterday.Date - StartDate.Date).Days;
        }

        // Convert an integer value to Base28 string
        private static string DecimalToBase28(long value)
        {
            string result = "";

            while (value > 0)
            {
                long remainder = value % 28;
                value = (value - remainder) / 28;
                result = dictionary[(int)remainder] + result;
            }

            return result;
        }

        // Convert a Base28 string to an integer value
        private static long Base28ToDecimal(string baseVal)
        {
            long value = 0;
            int posizione = baseVal.Length;

            for (int i = 0; i < baseVal.Length; i++)
            {
                int valpos = dictionary.IndexOf(baseVal[i]);
                if (valpos < 0 || valpos > 27)
                {
                    return value;
                }
                posizione -= 1;
                value += valpos * (long)Math.Pow(28, posizione);
            }

            return value;
        }

        // Reverse a string using byte xor (faster than using a temporary variable)
        private static string ReverseXor(string s)
        {
            if (s == null) return null;
            char[] charArray = s.ToCharArray();
            int len = s.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }

            return new String(charArray);
        }

        // Get N characters from right of the "input" string
        private static string GetRight(string input, int n)
        {
            if (n <= 0)
            {
                return "";
            }
            else if (n > input.Length)
            {
                return input;
            }
            else
            {
                return input.Substring(input.Length - n, n);
            }
        }

        // Calculate data checksum
        private static string CalculateChecksum(string toCheck)
        {
            int total = 0;

            for (int i = 0; i < toCheck.Length; i++)
            {
                total += (int)toCheck[i] * (int)Math.Pow(3, toCheck.Length - i);
            }

            return GetRight("AAA" + DecimalToBase28(total), 3);
        }

        // Generate a promocode as string
        public static string GeneratePromocode(int site, int num_trans)
        {
            // Format input to a string. 30 is POS number.
            string start = site.ToString("0000") + "30" + daysDiff.ToString("0000") + num_trans.ToString("0000");

            // Reverse the string, add 1 to start and convert to base28 string.
            string promo = DecimalToBase28(Int64.Parse("1" + ReverseXor(start)));

            // Generate checksum
            string genCheck = CalculateChecksum(promo);

            // Add checksum to base28 string
            promo = promo.Insert(9, genCheck[0].ToString());
            promo = promo.Insert(6, genCheck[1].ToString());
            promo = promo.Insert(1, genCheck[2].ToString());

            // Return result
            return promo;

        }

        // Return a string that contains all informations about a promocode
        public static string DecodePromocode(string promo)
        {
            // Remove checksum
            promo = promo.Remove(1, 1);
            promo = promo.Remove(6, 1);
            promo = promo.Remove(9, 1);

            // Decode
            promo = ReverseXor(Base28ToDecimal(promo).ToString().Substring(1));

            // Extract date
            DateTime date = DateTime.ParseExact(startDay, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = date.AddDays(Int32.Parse(promo.Substring(6, 4)));

            // Return string result
            return "Location: " + promo.Substring(0, 4) + ", POS number: " + promo.Substring(4, 2) + ", Transaction ID: " + promo.Substring(10) + ", Date: " + date.ToString("yyyy-MM-dd");
        }
    }
}