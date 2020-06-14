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
    // JSON response for account points number
    class PointsResponse
    {
        private string response;

        // Get json string
        public PointsResponse(string json)
        {
            response = json;
        }

        // Async JSON parser
        public async Task<string> GetPoints()
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = await JsonDocument.ParseAsync(Extensions.StrToStream(response), options))
            {
                // Return points number as string
                return document.RootElement.GetProperty("response").GetProperty("data").GetProperty("points").ToString();
            }
        }
    }
}
