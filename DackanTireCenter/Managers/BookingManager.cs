using System;
using System.Collections.Generic;
using DackanTireCenter.Models;
using DackanTireCenter.Utilities;

namespace DackanTireCenter.Managers
{
    public class BookingManager
    {
        private readonly Dictionary<string, BookedService> bookedServices;
        public readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots;
        private readonly NewBooking newBooking;
        private readonly UpdateBooking updateBooking;
        private readonly RemoveBooking removeBooking;
        private readonly ServiceManager serviceManager;

        public BookingManager()
        {
            bookedServices = new Dictionary<string, BookedService>();
            dailyBookingSlots = new Dictionary<DateOnly, Dictionary<TimeOnly, bool>>();
            serviceManager = new ServiceManager(); // Moved before newBooking initialization
            newBooking = new NewBooking(serviceManager, bookedServices, dailyBookingSlots); 
            updateBooking = new UpdateBooking(bookedServices, dailyBookingSlots);
            removeBooking = new RemoveBooking(bookedServices, dailyBookingSlots);
        }
        

        public Dictionary<TimeOnly, bool> GetDailySlots(DateOnly date)
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

        public void ShowBookedService(string carPlate)
        {
            if (bookedServices.TryGetValue(carPlate, out var service))
            {
                Console.WriteLine($"Bokad tjänst för {carPlate}: {service}");
            }
            else
            {
                Console.WriteLine($"Ingen bokning hittades för fordonet med registreringsnummer: {carPlate}");
            }
        }
        
        public List<Service> GetAvailableServices()
        {
            return serviceManager.GetAvailableServices();
        }

        public bool IsServiceBookedForCar(string carPlate, string serviceName)
        {
            return bookedServices.Values.Any(b =>
                b.CarPlate == carPlate &&
                b.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }

        public void HandleNewBooking(int choice)
        {
            newBooking.HandleBooking(choice);
        }

        public void UpdateBooking(string carPlate)
        {
            updateBooking.UpdateExistBooking(carPlate);
        }

        public void RemoveBookingByCar(string carPlate)
        {
            removeBooking.RemoveBookingByCar(carPlate);
        }

        public void BookService(string serviceName, decimal price, string carPlate, string name, string email, string phone, DateTime time)
        {
            var booking = new BookedService(serviceName, price, carPlate, name, email, phone, time);

            if (!bookedServices.ContainsKey(carPlate))
            {
                bookedServices[carPlate] = booking;
            }
            else
            {
                // Check if the same service is already booked for the same car and time
                if (bookedServices[carPlate].ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                    bookedServices[carPlate].BookingTime == time)
                {
                    Console.WriteLine("Fel: Samma tjänst är redan bokad för detta registreringsnummer och tid.");
                    return;
                }
                else
                {
                    bookedServices[carPlate] = booking; // Overwrite the existing booking
                }
            }

            var slots = GetDailySlots(DateOnly.FromDateTime(time));
            slots[TimeOnly.FromDateTime(time)] = true;

            Console.WriteLine($"Tjänsten '{serviceName}' har bokats för {carPlate} den {time:yyyy-MM-dd HH:mm}.");
        }

        public Dictionary<string, BookedService> GetAllBookings()
        {
            return bookedServices;
        }

        public void AddService(string serviceName, decimal price)
        {
            serviceManager.AddService(serviceName, price);
        }

    
        
        public void UpdateServiceName(string oldName, string newName)
        {
            serviceManager.UpdateServiceName(oldName, newName);
        }


        
        public void UpdateServicePrice(string serviceName, decimal newPrice)
        {
            serviceManager.UpdateServicePrice(serviceName, newPrice);
        }
        
        
        public void DeleteService(string serviceName)
        {
            serviceManager.DeleteService(serviceName);
        }

        public bool ServiceExists(string serviceName)
        {
            return serviceManager.ServiceExists(serviceName);
        }
    }
}
