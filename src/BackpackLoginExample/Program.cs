using System;
using BackpackLoginExample.Settings;
using HedgehogSoft.BackpackLogin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackpackLoginExample
{
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            Console.WriteLine("Press a key to start login process.");
            Console.ReadKey();
            Loader.LoadSettings();
            Console.WriteLine($"{ConsoleSettings.Instance["username"]} will login to backpack.tf");
            var backpackLoginClient = new BackpackLoginClient();
            var cookieContainer = backpackLoginClient.Login(
                ConsoleSettings.Instance["username"],
                ConsoleSettings.Instance["password"],
                ConsoleSettings.Instance["sharedSecret"]);
            Console.WriteLine($"Login successful. Found {cookieContainer.Count} Cookies.");
        }
    }
}
