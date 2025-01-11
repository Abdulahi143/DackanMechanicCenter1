using System;

namespace DackanTireCenter.Utilities
{
    public static class DateHelper
    {
        // Check if the given date falls on a weekend (Saturday or Sunday)
        public static bool IsWeekend(DateOnly date)
        {
            var day = date.DayOfWeek;
            return day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
        }
        
        public static bool IsPastDate(DateOnly date)
        {
            return date < DateOnly.FromDateTime(DateTime.Now);
        }

        // Get the next working day (skip weekends)
        public static DateOnly GetNextWorkingDay(DateOnly date)
        {
            do
            {
                date = date.AddDays(1);
            } while (IsWeekend(date));
            return date;
        }

        public static DateOnly GetFirstAvailableWorkingDay(DateOnly date)
        {
            return GetNextWorkingDay(date); // Simply use GetNextWorkingDay
        }
    }
}