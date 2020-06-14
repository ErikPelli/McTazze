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
using System.Threading.Tasks;

namespace McTazze.webRequest
{
    class McTool
    {
        // Async method that insert a coupon code into an account
        public static async Task<Tuple<string, string>> insert(jsonClasses.Account user, int index, coupon.Coupon gen, int number)
        {
            var httpSend = new McSend();

            // Number of coupons with 0 points inside.
            int zeroPoints = 0;
            // Number of coupons already inserted (check for another bot).
            int alreadyInserted = 0;
            // A start txId (nased on current cycle iteration).
            int txId = (index * 20) + 1;

            // Log
            Console.WriteLine("Recharching account N. " + number);

            // Cycle that lasts until the account is full
            while (true)
            {
                // If too many fails, change location and reset variables
                if (alreadyInserted >= 8 || txId > 150 || zeroPoints >= 9)
                {
                    zeroPoints = 0;
                    alreadyInserted = 0;
                    txId = (index * 20) + 1;
                    gen.ChangeLocation();
                }
                
                // Insert generated coupon to the account
                jsonClasses.PromoResponse promoInfo;
                try
                {
                    promoInfo = new jsonClasses.PromoResponse(await httpSend.PostPromo(gen.GetTazza(txId), user));
                }
                catch
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Network error. Retrying.");

                    // Restart cycle
                    continue;
                }

                // If user is unauthorized, skip this user.
                if (await promoInfo.IsUnauthorized())
                {
                    Console.WriteLine("Unauthorized");
                    break;
                }
                else if (await promoInfo.GetStatusCode() == 200)
                {
                    // if status code is ok, continue, resetting the alreadyInserted variable.
                    alreadyInserted = 0;
                    txId += 1;

                    // Get coupon status
                    var status = await promoInfo.GetPromoStatus();

                    // Check for status number
                    if (status == "2")
                    {
                        if (promoInfo.HasProducts())
                        {
                            // If ok and there are points, reset zeroPoints variable and print coupon points.
                            zeroPoints = 0;
                            Console.WriteLine("Coupon points: " + await promoInfo.GetCouponPoints());
                        }
                        else
                        {
                            // If is ok but there aren't products, zeroPoints += 1
                            zeroPoints += 1;
                            Console.WriteLine("Non sono presenti prodotti aderenti all'iniziativa."); // Correspondent error code on website
                        }
                    }
                    else if (status == "10")
                    {
                        Console.WriteLine("Codice scontrino non approvato, assicurati di averlo digitato correttamente."); // Correspondent error code on website
                        zeroPoints += 1;
                    }
                    else if (status == "6")
                    {
                        Console.WriteLine("This restaurant doesn't support this initiative.");

                        // Change location
                        alreadyInserted = 0;
                        zeroPoints = 0;
                        txId = (index * 20) + 1;
                        gen.ChangeLocation();
                    }
                    else
                    {
                        Console.WriteLine("Unknown status: " + status);
                        Console.WriteLine("LOG: " + promoInfo.GetResponse());
                    }
                }
                else if (await promoInfo.GetErrorCode() == "existing_receipt" || await promoInfo.GetErrorCode() == "wrong_format")
                {
                    alreadyInserted += 1;
                    txId += 1;
                    Console.WriteLine("Risultato: Codice già esistente!");
                }
                else if (await promoInfo.GetErrorCode() == "too_many_receipt" || await promoInfo.GetErrorCode() == "throttle_limit")
                {
                    // Account is full so exit
                    Console.WriteLine("Result: Account full!");
                    break;
                }
                else
                {
                    // Unknown, log.
                    Console.WriteLine(promoInfo.GetResponse());
                }

                // Delay for next iteration (to avoid ban)
                await Task.Delay(100);
            }

            // Save new cookies
            await Task.Delay(300);
            _ = await httpSend.GetCookies(user);

            // Get points number as string
            string points;
            try
            {
                var pointsResponse = new jsonClasses.PointsResponse(await httpSend.GetPoints(user.bearer, user.mc4u, user.token));
                points = await pointsResponse.GetPoints();
            }
            catch {
                points = "";
            }
            
            // Return result (email and points)
            return Tuple.Create(user.email, points);
        }
    }
}
