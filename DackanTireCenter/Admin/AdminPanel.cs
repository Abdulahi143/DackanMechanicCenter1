using System;
using System.Collections.Generic;
using DackanTireCenter.Models;
using DackanTireCenter.Managers;
using DackanTireCenter.Utilities;
namespace DackanTireCenter.Admin
{
    public class AdminPanel
    {
        private readonly BookingManager bookingManager;

        public AdminPanel(BookingManager bookingManager)
        {
            this.bookingManager = bookingManager;
        }

        public void ShowMenu()
        {
            int choice;

            do
            {
                Console.WriteLine("\nAdministratörspanel");
                Console.WriteLine("1. Visa alla bokade tjänster");
                Console.WriteLine("2. Boka en tid åt en kund");
                Console.WriteLine("3. Ta bort en bokning");
                Console.WriteLine("4. Uppdatera en boknings datum eller tid");
                Console.WriteLine("5. Hantera tjänster");
                Console.WriteLine("6. Tillbaka till huvudmenyn");
                Console.Write("Välj ett alternativ (1-6): ");

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                         ShowAllBookings();
                            break;
                        case 2:
                            NewBookingByAdmin();
                            break;
                        case 3:
                            DeleteBooking();
                            break;
                        case 4:
                            UpdateBooking();
                            break;
                        case 5:
                            ManageServices();
                            break;
                        case 6:
                            Console.WriteLine("Återgår till huvudmenyn.");
                            return;
                        default:
                            Console.WriteLine("Ogiltigt val.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt val.");
                }
            } while (choice != 6);
        }

        private void ShowAllBookings()
        {
            Console.WriteLine("\nVisa alla bokade tjänster:");
            Console.WriteLine("1. Visa dagens bokningar");
            Console.WriteLine("2. Sök bokningar för ett specifikt datum");
            Console.WriteLine("3. Visa alla bokningar");
            Console.Write("Välj ett alternativ (1-4): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
 
                    switch (choice)
                    {
                        case 1:
                            ShowTodaysBookings();
                            break;
                        case 2:
                            ShowBookingsForSpecificDate();
                            break;
                        case 3:
                            ShowAllBookingsRegardlessOfDate();
                            break;
                        case 4:
                            Console.WriteLine("Återgå till adminmeny!");
                            return;
                        default:
                            Console.WriteLine("Ogiltigt val.");
                            break;
                    }
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }
        }
        
        private void ShowTodaysBookings()
        {
            Console.WriteLine("\nDagens bokningar:");
            var todaysBookings = bookingManager.GetAllBookings()
                .Where(b => DateOnly.FromDateTime(b.Value.BookingTime) == DateOnly.FromDateTime(DateTime.Now))
                .ToList();

            if (todaysBookings.Count > 0)
            {
                foreach (var booking in todaysBookings)
                {
                    Console.WriteLine(booking.Value);
                }
            }
            else
            {
                Console.WriteLine("Inga bokningar idag.");
            }
        }

        private void ShowBookingsForSpecificDate()
        {
            Console.Write("Ange datum (ÅÅÅÅ-MM-DD): ");
            if (DateOnly.TryParse(Console.ReadLine(), out DateOnly date))
            {
                Console.WriteLine($"\nBokningar för {date:yyyy-MM-dd}:");
                var bookingsForDate = bookingManager.GetAllBookings()
                    .Where(b => DateOnly.FromDateTime(b.Value.BookingTime) == date)
                    .ToList();

                if (bookingsForDate.Count > 0)
                {
                    foreach (var booking in bookingsForDate)
                    {
                        Console.WriteLine(booking.Value);
                    }
                }
                else
                {
                    Console.WriteLine("Inga bokningar för detta datum.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt datumformat.");
            }
        }

        private void ShowAllBookingsRegardlessOfDate()
        {
            Console.WriteLine("\nAlla bokningar:");
            var allBookings = bookingManager.GetAllBookings();
            if (allBookings.Count > 0)
            {
                foreach (var booking in allBookings)
                {
                    Console.WriteLine(booking.Value);
                }
            }
            else
            {
                Console.WriteLine("Inga bokningar ännu.");
            }
        }
   private void NewBookingByAdmin()
{
    Console.WriteLine("\nBoka en tid åt en kund:");

    // Get booking date and time first
    Console.WriteLine("\nVill du ange ett specifikt datum eller ta den första tillgängliga tiden?");
    Console.WriteLine("1. Välj ett datum.");
    Console.WriteLine("2. Första tillgängliga tid.");
    Console.Write("Välj 1 eller 2: ");

    DateOnly date;
    if (int.TryParse(Console.ReadLine(), out int choice))
    {
        switch (choice)
        {
            case 1:
                Console.Write("Ange ett datum i formatet ÅÅÅÅ-MM-DD: ");
                if (!DateOnly.TryParse(Console.ReadLine(), out date))
                {
                    Console.WriteLine("Ogiltigt datumformat.");
                    return;
                }
                break;
            case 2:
                date = DateHelper.GetFirstAvailableWorkingDay(DateOnly.FromDateTime(DateTime.Now));
                break;
            default:
                Console.WriteLine("Ogiltigt val.");
                return;
        }
    }
    else
    {
        Console.WriteLine("Ogiltigt val.");
        return;
    }

    if (DateHelper.IsWeekend(date))
    {
        Console.WriteLine("Valt datum är en helgdag. Ange ett annat datum.");
        return;
    }

    // Show available time slots
    var slots = bookingManager.GetDailySlots(date);
    int index = 1;
    var slotList = new List<TimeOnly>();
    foreach (var slot in slots)
    {
        string status = slot.Value ? "Bokad" : "Ledig";
        Console.WriteLine($"{index}. {slot.Key:HH\\:mm} - {status}");
        if (!slot.Value)
        {
            slotList.Add(slot.Key);
        }
        index++;
    }

    if (slotList.Count == 0)
    {
        Console.WriteLine("Inga lediga tider detta datum.");
        return;
    }

    Console.Write("Välj en ledig tid genom att ange dess nummer: ");
    if (int.TryParse(Console.ReadLine(), out int slotIndex) && slotIndex > 0 && slotIndex <= slotList.Count)
    {
        TimeOnly selectedTime = slotList[slotIndex - 1];

        // Display the selected time to the admin
        Console.WriteLine($"Vald tid: {selectedTime:HH\\:mm}");

        // Get car plate number
        Console.Write("Ange bilens registreringsnummer: ");
        string carPlate = Console.ReadLine()?.Trim().ToUpper();

        // Get available services
        var availableServices = bookingManager.GetAvailableServices();

        // Display available services
        Console.WriteLine("Tillgängliga tjänster:");
        for (int i = 0; i < availableServices.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableServices[i].Name} - {availableServices[i].Price} SEK");
        }

        // Get service selection from admin
        Console.Write("Välj en tjänst genom att ange dess nummer: ");
        if (int.TryParse(Console.ReadLine(), out int serviceIndex) && serviceIndex > 0 && serviceIndex <= availableServices.Count)
        {
            var selectedService = availableServices[serviceIndex - 1];

            // Check if the service is already booked for the car
            if (bookingManager.IsServiceBookedForCar(carPlate, selectedService.Name))
            {
                Console.WriteLine($"Bilen med registreringsnummer {carPlate} har redan bokat tjänsten '{selectedService.Name}'. Var vänlig och uppdatera tjänsten eller uppdatera den befintliga!");
                Console.WriteLine("Vänligen välj en annan tjänst eller tid, eller uppdatera den befintliga bokningen.");
                return;
            }

            // Get customer details
            Console.Write("Ange kundens namn: ");
            string customerName = Console.ReadLine();
            Console.Write("Ange kundens e-postadress: ");
            string email = Console.ReadLine();
            Console.Write("Ange kundens telefonnummer: ");
            string phone = Console.ReadLine();

            // Book the service
            DateTime bookingTime = date.ToDateTime(selectedTime);
            bookingManager.BookService(selectedService.Name, selectedService.Price, carPlate, customerName, email, phone, bookingTime);
            Console.WriteLine("Kundens tid är bokad!");
        }
        else
        {
            Console.WriteLine("Ogiltigt val av tjänst.");
        }
    }
    else
    {
        Console.WriteLine("Ogiltigt val av tid.");
    }
}

        private void DeleteBooking()
        {
            Console.Write("Ange registreringsnummer för bilen vars bokning ska tas bort: ");
            string carPlate = Console.ReadLine()?.Trim().ToUpper();
            bookingManager.RemoveBookingByCar(carPlate);
        }

        private void UpdateBooking()
        {
            Console.WriteLine("\nAlla bokade tjänster:");
            foreach (var booking in bookingManager.GetAllBookings())
            {
                Console.WriteLine(booking.Value);
            }

            Console.Write("Ange registreringsnummer för bilen vars bokning ska uppdateras: ");
            string carPlate = Console.ReadLine()?.Trim().ToUpper();

            // Call UpdateExistBooking from the updateBooking instance
            bookingManager.UpdateBooking(carPlate);
        }

        private void ManageServices()
        {
            int choice;
            do
            {
                Console.WriteLine("\nHantera tjänster:");
                Console.WriteLine("1. Lägg till tjänst");
                Console.WriteLine("2. Uppdatera tjänst");
                Console.WriteLine("3. Ta bort tjänst");
                Console.WriteLine("4. Tillbaka till adminpanelen");
                Console.Write("Välj ett alternativ (1-4): ");

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddService();
                            break;
                        case 2:
                            UpdateService();
                            break;
                        case 3:
                            DeleteService();
                            break;
                        case 4:
                            Console.WriteLine("Återgår till adminpanelen.");
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt val.");
                }
            } while (choice != 4);
        }


        private void AddService()
        {
            Console.Write("Ange namnet på den nya tjänsten: ");
            string serviceName = Console.ReadLine();
            Console.Write("Ange priset för den nya tjänsten: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                bookingManager.AddService(serviceName, price);
                Console.WriteLine($"Tjänsten '{serviceName}' har lagts till.");
            }
            else
            {
                Console.WriteLine("Ogiltigt prisformat.");
            }
        }

      private void UpdateService()
{
    var services = bookingManager.GetAvailableServices();

    Console.WriteLine("\nNuvarande tjänster:");
    for (int i = 0; i < services.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {services[i].Name} - {services[i].Price} SEK");
    }

    Console.Write("Välj en tjänst att uppdatera genom att ange dess nummer: ");
    if (int.TryParse(Console.ReadLine(), out int serviceIndex) && serviceIndex > 0 && serviceIndex <= services.Count)
    {
        var selectedService = services[serviceIndex - 1];

        Console.WriteLine($"1. Vill du byta {selectedService.Name}");
        Console.WriteLine($"2. Ändra {selectedService.Name}s pris");
        Console.Write("Välj ett alternativ (1-2): ");

        if (int.TryParse(Console.ReadLine(), out int updateChoice))
        {
            switch (updateChoice)
            {
                case 1:
                    Console.Write("Ange det nya namnet på tjänsten: ");
                    string newName = Console.ReadLine();
                    bookingManager.UpdateServiceName(selectedService.Name, newName); 
                    Console.WriteLine($"Tjänsten '{selectedService.Name}' har bytt namn till '{newName}'.");
                    break;

                case 2:
                    Console.Write("Ange det nya priset för tjänsten: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                    {
                        bookingManager.UpdateServicePrice(selectedService.Name, newPrice); // Call UpdateServicePrice
                        Console.WriteLine($"Priset för '{selectedService.Name}' har uppdaterats till {newPrice} SEK.");
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt prisformat.");
                    }
                    break;

                default:
                    Console.WriteLine("Ogiltigt val.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Ogiltigt val.");
        }
    }
    else
    {
        Console.WriteLine("Ogiltigt val av tjänst.");
    }
}

        private void DeleteService()
        {
            Console.Write("Ange namnet på tjänsten som ska tas bort: ");
            string serviceName = Console.ReadLine();

            // Check if the service exists before attempting to delete
            if (!bookingManager.ServiceExists(serviceName))
            {
                Console.WriteLine($"Tjänsten '{serviceName}' hittades inte.");
                return;
            }

            bookingManager.DeleteService(serviceName);
            Console.WriteLine($"Tjänsten '{serviceName}' har tagits bort.");
        }
    }
}