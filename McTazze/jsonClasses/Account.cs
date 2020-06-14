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

namespace McTazze.jsonClasses
{
    // Object that represent a single account
    class Account
    {
        public string bearer { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public string mc4u { get; set; }

        public override string ToString()
        {
            return String.Format("['Bearer': '{0}', 'Token': '{1}', 'Email': '{2}', 'Mc4u': '{3}']", bearer, token, email, mc4u);
        }
    }
}
