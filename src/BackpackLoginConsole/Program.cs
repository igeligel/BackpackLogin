using System;
using BackpackLogin;

namespace BackpackLoginConsole
{
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            Console.ReadKey();
            BackpackLoginClient backpackLoginClient = new BackpackLoginClient();
            backpackLoginClient.Login("", "", "");
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
