using DackanTireCenter.Admin;
using DackanTireCenter.Authenticate;
using DackanTireCenter.Managers;
using DackanTireCenter.Utilities;

namespace DackanTireCenter
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nHej! Välkommen till Däckan Tire Center!");
            BookingManager bookingManager = new BookingManager();
            int choice;

            do
            {
                Console.WriteLine("\n1. Visa dina bokade tider.");
                Console.WriteLine("2. Boka en ny tid.");
                Console.WriteLine("3. Uppdatera en bokad tid.");
                Console.WriteLine("4. Avboka en bokad tid.");
                Console.WriteLine("5. Logga in som administratör.");
                Console.WriteLine("6. Avsluta programmet.");
                Console.Write("\nVälj ett av alternativen 1–6: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Ogiltigt val. Vänligen ange en siffra mellan 1 och 6.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Ange bilens registreringsnummer (t.ex. ABC123): ");
                        string carPlate = Console.ReadLine()?.Trim().ToUpper();
                        bookingManager.ShowBookedService(carPlate);
                        break;

                    case 2:
                        Console.WriteLine("\nVill du ange ett specifikt datum eller ta den första tillgängliga tiden?");
                        Console.WriteLine("1. Välj ett datum.");
                        Console.WriteLine("2. Första tillgängliga tid.");
                        Console.Write("\nVälj 1 eller 2: ");

                        if (int.TryParse(Console.ReadLine(), out int bookingChoice))
                        {
                            bookingManager.HandleNewBooking(bookingChoice);
                        }
                        else
                        {
                            Console.WriteLine("Ogiltigt val. Försök igen.");
                        }
                        break;

                    case 3:
                        Console.Write("Ange bilens registreringsnummer (t.ex. ABC123): "); 
                        carPlate = Console.ReadLine()?.Trim().ToUpper();
                        bookingManager.UpdateBooking(carPlate);
                        break;

                    case 4:
                        Console.Write("Ange bilens registreringsnummer (t.ex. ABC123): ");
                        carPlate = Console.ReadLine()?.Trim().ToUpper();
                        bookingManager.RemoveBookingByCar(carPlate); // Call RemoveBookingByCar in bookingManager
                        break;

                    case 5:
                        AdminSignIn adminSignIn = new AdminSignIn();
                        if (adminSignIn.SignIn())
                        {
                            Console.WriteLine("Välkommen till admin panelen");
                            AdminPanel adminPanel = new AdminPanel(bookingManager);
                            adminPanel.ShowMenu(); // Transition to the admin panel
                        }

                        break;
                    case 6:
                        Console.WriteLine("Tack för att du använder Däckan Tire Center. Ha en bra dag!");
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val. Försök igen.");
                        break;
                }

            } while (choice != 6);
        }
    }
}