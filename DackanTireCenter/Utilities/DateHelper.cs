using System;

namespace DackanTireCenter.Utilities
{
    public static class DateHelper
    {
        // Här kontrollerar vi så att datumet som användaren matat in är inte en helgdag som lör/sön
        public static bool IsWeekend(DateOnly date)
        {
            var day = date.DayOfWeek;
            return day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
        }
        
        public static bool IsPastDate(DateOnly date)
        {
            return date < DateOnly.FromDateTime(DateTime.Now);
        }

        // Hoppa över till nästa arbetsdag om användare valt helgdag
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
            return GetNextWorkingDay(date); // Här retuneras närmaste arbetsdag
        }
    }
}