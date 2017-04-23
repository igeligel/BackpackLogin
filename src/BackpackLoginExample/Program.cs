using System;
using BackpackLoginExample.Settings;
using HedgehogSoft.BackpackLogin;

namespace BackpackLoginExample
{
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            Console.ReadKey();
            Loader.LoadSettings();
            Console.WriteLine($"Username {ConsoleSettings.Instance["username"]}");
            var backpackLoginClient = new BackpackLoginClient();
            backpackLoginClient.Login(
                ConsoleSettings.Instance["username"],
                ConsoleSettings.Instance["password"],
                ConsoleSettings.Instance["sharedSecret"]);
            Console.ReadKey();
        }
    }
}
