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

namespace McTazze.coupon
{
    // Object that represent a coupon generator from random location
    class Coupon
    {
        private (int, string) location;
 
        // Get random mcdonald location
        public Coupon()
        {
            location = Locations.GetLocation();
        }

        // Change location with a random one
        public void ChangeLocation()
        {
            location = Locations.GetLocation();
        }

        // Get coupon in this location from a treansaction id
        public string GetTazza(int txId)
        {
            // Generate a new coupon and print informations
            var coupon = Promo.GeneratePromocode(location.Item1, txId);
            Console.WriteLine("Coupon generated: " + coupon + ". Location: " + location.Item2 + "[" + location.Item1 + "]");
            return coupon;
        }
    }
}
