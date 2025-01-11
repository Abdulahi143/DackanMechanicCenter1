using System;
using System.Collections.Generic;
using System.Linq;
using DackanTireCenter.Models;

namespace DackanTireCenter.Managers
{
    public class RemoveBooking
    {
        private readonly Dictionary<string, BookedService> bookedServices; // Add bookedServices
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots; // Add dailyBookingSlots

        public RemoveBooking(Dictionary<string, BookedService> bookedServices, Dictionary<DateOnly, Dictionary<TimeOnly, bool>> dailyBookingSlots)
        {
            this.bookedServices = bookedServices;
            this.dailyBookingSlots = dailyBookingSlots;
        }

        public void RemoveBookingByCar(string carNumber) // Add carNumber parameter
        {
            if (!bookedServices.ContainsKey(carNumber) || bookedServices[carNumber] == null)
            {
                Console.WriteLine("Inga bokningar hittades för detta registreringsnummer.");
                return;
            }

            var bookings = bookedServices[carNumber]; // Get the booking from bookedServices

            // Display the booking to the user
            Console.WriteLine($"Bokning för bil {carNumber}:");
            Console.WriteLine($"Datum: {bookings.BookingTime:yyyy-MM-dd}, Tjänst: {bookings.ServiceName}");

            // Confirmation before deleting
            Console.Write("Vill du avboka denna bokning? (ja/nej): ");
            string confirmation = Console.ReadLine()?.ToLower();
            if (confirmation != "ja")
            {
                Console.WriteLine("Avbokning avbruten.");
                return;
            }

            // Remove the booking from bookedServices and free the time slot
            bookedServices.Remove(carNumber);
            var slots = dailyBookingSlots[DateOnly.FromDateTime(bookings.BookingTime)];
            slots[TimeOnly.FromDateTime(bookings.BookingTime)] = false;

            Console.WriteLine("Bokningen har avbokats.");
        }
    }
}