using System;

namespace DackanTireCenter.Authenticate
{
    public class AdminSignIn
    {
        public bool SignIn()
        {
            Console.WriteLine("Logga in som admin");
            Console.Write("Skriv ditt användernamn: ");
            string username = Console.ReadLine().ToLower().Trim();
            Console.Write("Ange administratörslösenord: ");
            string password = Console.ReadLine().ToLower().Trim();

            if (username == "admin" && password == "admin123") 
            {
                Console.WriteLine("Inloggning lyckades!");
                return true;
            }
            else
            {
                Console.WriteLine("Fel lösenord.");
                return false;
            }
        }
    }
}