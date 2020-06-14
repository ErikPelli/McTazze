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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace McTazze.webRequest
{
    // An async coupon generator
    class McThread
    {
        private jsonClasses.Account[] users;

        // Load accounts
        public McThread(jsonClasses.Account[] users)
        {
            this.users = users;
        }

        // Running thread
        public async Task<bool> Thread() {
            // List of async tasks
            var tasks = new List<Task<Tuple<string, string>>>();

            int locChange = 0;
            var generator = new coupon.Coupon();

            int iteration = 1;
            foreach (var user in users)
            {
                // Create a new coupon task for this account
                tasks.Add(McTool.insert(user, locChange, generator, iteration));

                // Every 5 accounts, change location
                if (locChange < 5)
                {
                    locChange++;
                }
                else
                {
                    locChange = 0;
                    generator = new coupon.Coupon();
                }

                // i+1
                iteration++;

                // Delay for next account
                await Task.Delay(600);
            }

            // Foreach completed task, append result to "accounts.txt" file
            foreach(var i in await Task.WhenAll(tasks)){
                using (StreamWriter w = File.AppendText("accounts.txt"))
                {
                    w.WriteLine(i.Item1 + "|" + i.Item2);
                }
            }
            
            return true;
        }
    }
}