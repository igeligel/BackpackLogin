using System;
using BackpackLogin.API;

namespace BackpackLogin.Main
{
    /// <summary>
    /// Program class to initiate the program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method of the program.
        /// </summary>
        /// <param name="args">Arguments which will be provided. There is no support for that right now.</param>
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            Console.ReadKey();
            // Create instance of the login.
            var backpackLogin = new Login();
            // Login with two factor code.
            backpackLogin.DoLogin("steamusername", "steampasswort", "twofactorcode");
            // Login without two factor code.
            backpackLogin.DoLogin("steamusername", "steampassword");
            Console.ReadKey();
        }
    }
}
