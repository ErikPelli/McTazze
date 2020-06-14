using System.IO;
using System.Text.Json;

namespace McTazze
{
    // Class that represent all McDonald's users
    class Users
    {
        public static jsonClasses.Account[] users { get; set;}

        // Read users from "accounts.json" file
        static Users()
        {
            string jsonString = File.ReadAllText("accounts.json");
            users = JsonSerializer.Deserialize<jsonClasses.Account[]>(jsonString);
        }

        // Write result to new file "accountsNew.txt"
        public static void Output()
        {
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText("accountsNew.txt", json);
        }
    }
}
