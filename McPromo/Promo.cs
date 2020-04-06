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

namespace McPromo
{
    class Promo
    {
        private readonly static int daysDiff;
        private readonly static string dictionary = "ABCDEFGHIJKLMNOPRSTUWXZ45679";

        static Promo()
        {
            DateTime Yesterday = DateTime.Today.AddDays(-1);
            DateTime StartDate = DateTime.ParseExact("2014-12-31", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            daysDiff = (Yesterday.Date - StartDate.Date).Days;
        }

        private static string decimalToBase28(long value)
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

        private static long base28ToDecimal(string baseVal)
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

        private static string reverseXor(string s)
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

        private static string right(string input, int n)
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
        private static string checksum(string toCheck)
        {
            int total = 0;

            for (int i = 0; i < toCheck.Length; i++)
            {
                total += (int)toCheck[i] * (int)Math.Pow(3, toCheck.Length - i);
            }

            return right("AAA" + decimalToBase28(total), 3);
        }

        public static string generatePromocode(int site, int num_trans)
        {
            string start = site.ToString("0000") + "30" + daysDiff.ToString("0000") + num_trans.ToString("0000");
            string promo = decimalToBase28(Int64.Parse("1" + reverseXor(start)));
            string genCheck = checksum(promo);

            promo = promo.Insert(9, genCheck[0].ToString());
            promo = promo.Insert(6, genCheck[1].ToString());
            promo = promo.Insert(1, genCheck[2].ToString());

            return promo;

        }

        public static string decodePromocode(string promo)
        {
            // remove checksum
            promo = promo.Remove(1, 1);
            promo = promo.Remove(6, 1);
            promo = promo.Remove(9, 1);
            // decode
            promo = reverseXor(base28ToDecimal(promo).ToString().Substring(1));
            // extract date
            DateTime date = DateTime.ParseExact("2014-12-31", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = date.AddDays(Int32.Parse(promo.Substring(6, 4)));
            // return results
            return "loc: " + promo.Substring(0, 4) + ", pos: " + promo.Substring(4, 2) + ", tx_id: " + promo.Substring(10) + ", date: " + date.ToString("yyyy-MM-dd");
        }
    }
}