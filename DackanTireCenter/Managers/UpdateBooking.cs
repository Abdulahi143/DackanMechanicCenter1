using System;
using System.Collections.Generic;
using DackanTireCenter.Models;
using DackanTireCenter.Utilities;

namespace DackanTireCenter.Managers
{
    public class UpdateBooking
    {
        private readonly Dictionary<string, BookedService> bookedServices;
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots;

        public UpdateBooking(Dictionary<string, BookedService> bookedServices, Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots)
        {
            this.bookedServices = bookedServices;
            this.dailyBookingSlots = dailyBookingSlots;
        }

        public void UpdateExistBooking(string carPlate)
        {
            if (bookedServices.TryGetValue(carPlate, out var service))
            {
                Console.WriteLine($"Bokad tjänst för {carPlate}");
                Console.WriteLine($"Tjänsten: {service}");
                Console.WriteLine("1. Uppdatera bilens registreringsnummer.");
                Console.WriteLine("2. Ändra tjänst.");
                Console.WriteLine("3. Ändra datum.");
                Console.WriteLine("4. Uppdatera namn.");
                Console.WriteLine("5. Uppdatera e-postadressen.");
                Console.WriteLine("6. Uppdatera mobilen.");
                Console.Write("Välj ett alternativ (1-6): ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            UpdateCarPlate(carPlate);
                            break;
                        case 2:
                            ChangeService(carPlate);
                            break;
                        case 3:
                            ChangeBookingDate(carPlate);
                            break;
                        case 4:
                            UpdateName(carPlate);
                            break;
                        case 5:
                            UpdateEmail(carPlate);
                            break;
                        case 6:
                            UpdatePhone(carPlate);
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
                Console.WriteLine($"Ingen bokning hittades för fordonet med registreringsnummer: {carPlate}");
            }
        }

        private void UpdateCarPlate(string oldCarPlate)
        {
            Console.Write("Ange det nya registreringsnumret: ");
            string newCarPlate = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(newCarPlate))
            {
                Console.WriteLine("Ogiltigt registreringsnummer.");
                return;
            }

            if (bookedServices.TryGetValue(newCarPlate, out var existingBooking) &&
                existingBooking.ServiceName.Equals(bookedServices[oldCarPlate].ServiceName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Fel: Tjänsten '{existingBooking.ServiceName}' är redan bokad för bilen med registreringsnummer {newCarPlate}.");
                return;
            }

            bookedServices[oldCarPlate].CarPlate = newCarPlate;
            bookedServices[newCarPlate] = bookedServices[oldCarPlate];
            bookedServices.Remove(oldCarPlate);

            Console.WriteLine($"Registreringsnumret har uppdaterats till {newCarPlate}.");
        }

        private void ChangeService(string carPlate)
        {
            List<Service> availableServices = new List<Service>
            {
                new Service("Oljebyte", 499),
                new Service("Däckbyte", 999),
                new Service("Bromsinspektion", 299)
            };

            Console.WriteLine("Tillgängliga tjänster:");
            for (int i = 0; i < availableServices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableServices[i].Name} - {availableServices[i].Price} SEK");
            }

            Console.Write("Välj en ny tjänst genom att ange dess nummer: ");
            if (int.TryParse(Console.ReadLine(), out int serviceIndex) && serviceIndex > 0 && serviceIndex <= availableServices.Count)
            {
                var selectedService = availableServices[serviceIndex - 1];

                if (IsServiceAlreadyBookedForCar(carPlate, selectedService.Name))
                {
                    Console.WriteLine("Tjänsten är redan bokad för den här bilen.");
                    return;
                }

                var booking = bookedServices[carPlate];
                booking.ServiceName = selectedService.Name;
                booking.Price = selectedService.Price;

                Console.WriteLine($"Tjänsten har ändrats till {selectedService.Name} för {carPlate}.");
            }
            else
            {
                Console.WriteLine("Ogiltigt val av tjänst.");
            }
        }

       private void ChangeBookingDate(string carPlate)
{
    var booking = bookedServices[carPlate];
    DateOnly originalDate = DateOnly.FromDateTime(booking.BookingTime);
    TimeOnly originalTime = TimeOnly.FromDateTime(booking.BookingTime);

    Console.WriteLine("Vill du ändra till ett annat datum eller bara tiden?");
    Console.WriteLine("1. Ändra bara tiden.");
    Console.WriteLine("2. Ändra till ett annat datum.");
    Console.Write("Välj ett alternativ (1-2): ");

    if (int.TryParse(Console.ReadLine(), out int choice))
    {
        DateOnly newDate = originalDate;
        TimeOnly newTime = originalTime;

        switch (choice)
        {
            case 1:
                ShowAvailableSlotsForChange(originalDate, originalTime, carPlate);
                break;

            case 2:
                while (true) // Loop until a valid date is chosen
                {
                    Console.Write("Ange det nya datumet (ÅÅÅÅ-MM-DD): ");
                    if (DateOnly.TryParse(Console.ReadLine(), out newDate))
                    {
                        if (DateHelper.IsPastDate(newDate))
                        {
                            Console.WriteLine("Du valde ett datum i det förflutna. Vänligen välj ett arbetsdag som inte är i det förflutna.");
                            continue; // Repeat the input
                        }
                        if (DateHelper.IsWeekend(newDate))
                        {
                            Console.WriteLine("Valt datum är en helgdag. Ange ett annat datum.");
                            continue; // Repeat the input
                        }
                        ShowAvailableSlotsForChange(newDate, originalTime, carPlate);
                        break; // Exit the loop if a valid date is chosen
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt datumformat. Försök igen.");
                    }
                }
                break;

            default:
                Console.WriteLine("Ogiltigt val.");
                return;
        }
    }
    else
    {
        Console.WriteLine("Ogiltigt val.");
    }
}

        private void ShowAvailableSlotsForChange(DateOnly date, TimeOnly originalTime, string carPlate)
        {
            Console.WriteLine($"Tillgängliga tider för {date:yyyy-MM-dd}:");
            var slots = GetDailySlots(date);
            int index = 1;
            var now = DateTime.Now;
            var slotList = new List<TimeOnly>();

            foreach (var slot in slots)
            {
                if (date == DateOnly.FromDateTime(now) && slot.Key <= TimeOnly.FromDateTime(now))
                {
                    continue;
                }

                if (slot.Value && slot.Key != originalTime)
                {
                    continue;
                }

                Console.WriteLine($"{index}. {slot.Key:HH\\:mm} - {(slot.Value ? "Bokad" : "Ledig")}");
                slotList.Add(slot.Key);
                index++;
            }

            if (slotList.Count == 0)
            {
                Console.WriteLine("Inga lediga tider detta datum.");
                return;
            }

            Console.Write("\nVälj en ledig tid genom att ange dess nummer: ");
            if (int.TryParse(Console.ReadLine(), out int slotChoice) && slotChoice > 0 && slotChoice <= slotList.Count)
            {
                TimeOnly selectedTime = slotList[slotChoice - 1];
                UpdateBookingTime(carPlate, date, selectedTime);
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }
        }

        private void UpdateBookingTime(string carPlate, DateOnly newDate, TimeOnly newTime)
        {
            var booking = bookedServices[carPlate];
            DateTime newBookingTime = newDate.ToDateTime(newTime);

            // Free the original time slot
            var originalSlots = GetDailySlots(DateOnly.FromDateTime(booking.BookingTime));
            TimeOnly originalTime = TimeOnly.FromDateTime(booking.BookingTime);
            originalSlots[originalTime] = false;

            // Book the new time slot
            var newSlots = GetDailySlots(newDate);
            newSlots[newTime] = true;

            // Update the booking
            booking.BookingTime = newBookingTime;
            Console.WriteLine($"Bokningen har uppdaterats till {newBookingTime:yyyy-MM-dd HH:mm}.");
        }

        private void UpdateName(string carPlate)
        {
            Console.Write("Ange det nya namnet: ");
            string newName = Console.ReadLine();
            bookedServices[carPlate].CustomerName = newName;
            Console.WriteLine("Namnet har uppdaterats.");
        }

        private void UpdateEmail(string carPlate)
        {
            Console.Write("Ange den nya e-postadressen: ");
            string newEmail = Console.ReadLine();
            bookedServices[carPlate].Email = newEmail;
            Console.WriteLine("E-postadressen har uppdaterats.");
        }

        private void UpdatePhone(string carPlate)
        {
            Console.Write("Ange det nya telefonnumret: ");
            string newPhone = Console.ReadLine();
            bookedServices[carPlate].Phone = newPhone;
            Console.WriteLine("Telefonnumret har uppdaterats.");
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