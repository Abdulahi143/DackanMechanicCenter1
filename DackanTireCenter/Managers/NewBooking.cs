using System;
using System.Collections.Generic;
using DackanTireCenter.Models;
using DackanTireCenter.Utilities;

namespace DackanTireCenter.Managers
{
    public class NewBooking
    {
        private readonly BookingManager bookingManager;
        private readonly Dictionary<string, BookedService> bookedServices;
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots;
        private readonly ServiceManager serviceManager;
        public NewBooking(ServiceManager serviceManager, 
            Dictionary<string, BookedService> bookedServices, 
            Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots) 
        {
            this.serviceManager = serviceManager;
            this.bookingManager = bookingManager; // Assign the bookingManager
            this.bookedServices = bookedServices;
            this.dailyBookingSlots = dailyBookingSlots;
        }

        public void HandleBooking(int choice)
        {
            switch (choice)
            {
                case 1:
                    Console.Write("Ange ett datum i formatet ÅÅÅÅ-MM-DD: ");
                    if (DateOnly.TryParse(Console.ReadLine(), out DateOnly userDate))
                    {
                        if (DateHelper.IsPastDate(userDate))
                        {
                            Console.WriteLine("Du valde ett datum i det förflutna. Vänligen välj ett arbetsdag som inte är i det förflutna.");
                            return; // Exit the method if the date is in the past
                        }
                        if (DateHelper.IsWeekend(userDate))
                        {
                            Console.WriteLine("Valt datum är en helgdag. Ange ett annat datum.");
                            return; // Exit the method if the date is a weekend
                        }
                        ShowAvailableSlots(userDate);
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt datumformat.");
                    }
                    break;

                case 2:
                    DateOnly firstAvailableDate = DateHelper.GetFirstAvailableWorkingDay(DateOnly.FromDateTime(DateTime.Now));
                    ShowAvailableSlots(firstAvailableDate);
                    break;

                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }

        public void ShowAvailableSlots(DateOnly date)
        {
            Console.WriteLine($"Tillgängliga tider för {date:yyyy-MM-dd}:");
            var slots = GetDailySlots(date);
            DateTime now = DateTime.Now;
            bool hasAvailableSlots = false;
            TimeOnly firstAvailableTime = default;

            foreach (var slot in slots)
            {
                if (date == DateOnly.FromDateTime(now) && slot.Key <= TimeOnly.FromDateTime(now))
                    continue;

                if (!slot.Value) // Om tiden inte är bokad
                {
                    hasAvailableSlots = true;
                    if (firstAvailableTime == default) // Om detta är den första tillgängliga tiden
                    {
                        firstAvailableTime = slot.Key; // Sätt den första tillgängliga tiden
                    }
                }

                string status = slot.Value ? "Fullbokad" : "Ledig";
                Console.WriteLine($"{slot.Key:HH\\:mm} - {status}");
            }

            if (!hasAvailableSlots)
            {
                Console.WriteLine("Inga tillgängliga tider detta datum. Letar efter nästa arbetsdag...");
                ShowAvailableSlots(DateHelper.GetNextWorkingDay(date));
                return;
            }

            // Välj automatiskt den första tillgängliga tiden
            Console.WriteLine($"\nFörsta tillgängliga tid: {firstAvailableTime:HH\\:mm}");
            Console.WriteLine("Bokar automatiskt denna tid...");

            // Fortsätt med bokningen med den första tillgängliga tiden
            HandleNewBooking(firstAvailableTime, date);
        }
        public void HandleNewBooking(TimeOnly time, DateOnly date)
        {
            // Get the available services from the BookingManager
            List<Service> availableServices = serviceManager.GetAvailableServices(); 
            Console.Write("Ange bilens registreringsnummer (t.ex. ABC123): ");
            string carPlate = Console.ReadLine()?.Trim().ToUpper();

            Console.WriteLine("Tillgängliga tjänster:");
            for (int i = 0; i < availableServices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableServices[i].Name} - {availableServices[i].Price} SEK");
            }

            Console.Write("Välj en tjänst genom att ange dess nummer: ");
            if (int.TryParse(Console.ReadLine(), out int serviceIndex) && serviceIndex > 0 && serviceIndex <= availableServices.Count)
            {
                var selectedService = availableServices[serviceIndex - 1];

                if (IsServiceAlreadyBookedForCar(carPlate, selectedService.Name))
                {
                    Console.WriteLine("Tjänsten är redan bokad för den här bilen.");
                    return;
                }

                Console.Write("Ange ditt namn: ");
                string name = Console.ReadLine();

                Console.Write("Ange din e-postadress: ");
                string email = Console.ReadLine();

                Console.Write("Ange ditt telefonnummer: ");
                string phone = Console.ReadLine();

                DateTime bookingTime = date.ToDateTime(time);
                BookService(selectedService.Name, selectedService.Price, carPlate, name, email, phone, bookingTime);
                Console.WriteLine("Bokningen är klar!");
            }
            else
            {
                Console.WriteLine("Ogiltigt val av tjänst.");
            }
        }

        private void BookService(string serviceName, decimal price, string carPlate, string name, string email, string phone, DateTime time)
        {
            if (bookedServices.ContainsKey(carPlate))
            {
                Console.WriteLine("Fel: Tjänsten är redan bokad för detta registreringsnummer.");
                return;
            }

            var booking = new BookedService(serviceName, price, carPlate, name, email, phone, time);
            bookedServices[carPlate] = booking;

            var slots = GetDailySlots(DateOnly.FromDateTime(time));
            slots[TimeOnly.FromDateTime(time)] = true;

            Console.WriteLine($"Tjänsten '{serviceName}' har bokats för {carPlate} den {time:yyyy-MM-dd HH:mm}.");
        }

        private Dictionary<TimeOnly, bool> GetDailySlots(DateOnly date)
        {
            if (!dailyBookingSlots.ContainsKey(date))
            {
                dailyBookingSlots[date] = new Dictionary<TimeOnly, bool>
                {
                    { new TimeOnly(8, 10), false },
                    { new TimeOnly(8, 55), false },
                    { new TimeOnly(9, 40), false },
                    { new TimeOnly(10, 25), false },
                    { new TimeOnly(11, 10), false },
                    { new TimeOnly(13, 0), false },
                    { new TimeOnly(13, 45), false },
                    { new TimeOnly(14, 30), false },
                    { new TimeOnly(15, 30), false },
                    { new TimeOnly(16, 15), false }
                };
            }
            return dailyBookingSlots[date];
        }

        private bool IsServiceAlreadyBookedForCar(string carPlate, string serviceName)
        {
            return bookedServices.TryGetValue(carPlate, out var service) && service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase);
        }
    }
}