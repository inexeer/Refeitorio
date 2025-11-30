using System;
using System.Collections.Generic;
using System.Linq;

namespace Refeitorio.Services
{
    public class BookingRules
    {
        public static bool CanBook(DateTime bookingDateTime)
        {
            var today = DateTime.Today;
            if (bookingDateTime.Date == today)
            {
                var deadline = today.AddHours(10);
                return DateTime.Now <= deadline;
            }

            return bookingDateTime.Date > today;
        }

        public static bool CanCancel(DateTime bookingDateTime)
        {
            var today = DateTime.Today;
            if (bookingDateTime.Date == today)
            {
                var deadline = today.AddHours(9).AddMinutes(30); // 09:30 AM
                return DateTime.Now <= deadline;
            }

            return bookingDateTime.Date > today;
        }

        public static bool HasConflictByDate(DateTime newBookingDate, IEnumerable<DateTime> existingBookingDates)
        {
            if (existingBookingDates == null) return false;
            return existingBookingDates.Any(d => d.Date == newBookingDate.Date);
        }
    }
}