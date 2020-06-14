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

using System.Text.Json;
using System.Threading.Tasks;

namespace McTazze.jsonClasses
{
    // Object that contains JSON response from coupon redeem
    class PromoResponse
    {
        private string response;

        public PromoResponse(string json)
        {
            response = json;
        }

        public string GetResponse()
        {
            return response;
        }

        // Return web request status
        public async Task<int> GetStatusCode()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                return document.RootElement.GetProperty("response").GetProperty("status_code").GetInt32();
            }
        }

        // Check if the request is "Unauthorized"
        public async Task<bool> IsUnauthorized()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                return document.RootElement.GetProperty("message").ToString() == "Unauthorized";
            }
        }

        // Check for http errors (in json)
        public async Task<string> GetErrorCode()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                return document.RootElement.GetProperty("response").GetProperty("error").GetProperty("message").ToString();
            }
        }

        // Get coupon status (from json)
        public async Task<string> GetPromoStatus()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                return document.RootElement.GetProperty("response").GetProperty("data").GetProperty("rawreceipt").GetProperty(char.ConvertFromUtf32(0) + "*" + char.ConvertFromUtf32(0) + "attributes").GetProperty("status").ToString();
            }
        }

        // CHeck if receipt has products bought
        public bool HasProducts()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = JsonDocument.Parse(response, options))
            {
                try
                {
                    _ = document.RootElement.GetProperty("response").GetProperty("data").GetProperty("rawreceipt").GetProperty(char.ConvertFromUtf32(0) + "*" + char.ConvertFromUtf32(0) + "products")[0];
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Get points nukmber from current coupon
        public async Task<string> GetCouponPoints()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                return document.RootElement.GetProperty("response").GetProperty("data").GetProperty("rawreceipt").GetProperty(char.ConvertFromUtf32(0) + "*" + char.ConvertFromUtf32(0) + "products")[0].GetProperty("rawproduct").GetProperty("quantity").ToString();
            }
        }

    }
}
