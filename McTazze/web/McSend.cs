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

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace McTazze.webRequest
{
    // Class that send 
    class McSend
    {
        // Get new cookie values and store them
        public async Task<bool> GetCookies(jsonClasses.Account user)
        {
            var handler = new HttpClientHandler { UseCookies = false };
            var client = new HttpClient(handler);

            // Add request headers
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("Host", "collectioning.mcdonalds.it");
            client.DefaultRequestHeaders.Add("Referer", "https://collectioning.mcdonalds.it/verify/otp");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0");
            client.DefaultRequestHeaders.Add("Cookie", user.mc4u);

            // Get webpage
            var response = await client.GetAsync("https://collectioning.mcdonalds.it/la-raccolta");

            try
            {
                // mc4u in the header
                user.mc4u = response.Headers.GetValues("Set-Cookie").First().Split(";")[0];

                // Parse webpage
                var page = await response.Content.ReadAsStringAsync();

                // Other cookies values
                user.bearer = page.Substring(page.IndexOf("APP.USER.user_laravel = '") + 25, 358);
                user.token = page.Substring(page.IndexOf("APP.TOKEN = \"") + 13, 40);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Redeem a coupon code for an account
        public async Task<string> PostPromo(string couponCode, jsonClasses.Account user)
        {
            var handler = new HttpClientHandler { UseCookies = false };
            var client = new HttpClient(handler);

            // Add request headers
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.bearer);
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Length", "69");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("DNT", "1");
            client.DefaultRequestHeaders.Add("Host", "collectioning.mcdonalds.it");
            client.DefaultRequestHeaders.Add("Origin", "https://collectioning.mcdonalds.it");
            client.DefaultRequestHeaders.Add("Referer", "https://collectioning.mcdonalds.it/inserisci-scontrino");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Cookie", user.mc4u);
            client.DefaultRequestHeaders.Add("X-CSRF-Token", user.token);

            // POST parameters
            var content = new Dictionary<string, string>
            {
                {"_token", user.token},
                {"receipt", couponCode}
            };

            // send request and return response string
            var response = await client.PostAsync("https://collectioning.mcdonalds.it/api/me/receipts", new FormUrlEncodedContent(content));
            return await response.Content.ReadAsStringAsync();
        }

        // Get points number for an account
        public async Task<string> GetPoints(string bearer, string mc4u, string token)
        {
            var handler = new HttpClientHandler { UseCookies = false };
            var client = new HttpClient(handler);

            // Add request headers
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearer);
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Length", "69");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            client.DefaultRequestHeaders.Add("Host", "collectioning.mcdonalds.it");
            client.DefaultRequestHeaders.Add("Origin", "https://collectioning.mcdonalds.it");
            client.DefaultRequestHeaders.Add("Referer", "https://collectioning.mcdonalds.it/inserisci-scontrino");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Cookie", mc4u);
            client.DefaultRequestHeaders.Add("X-CSRF-Token", token);

            // Get points number
            var response = await client.GetAsync("https://collectioning.mcdonalds.it/api/me/collections/3/wallet");

            // Check for errors
            if (response.IsSuccessStatusCode)
            {
                // Return string result
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new System.Exception("Error with the points web request.");
            }
        }
    }
}
