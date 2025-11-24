using Refeitorio.Models;
using System.Text.Json;

namespace Refeitorio.Services
{
    public class BookingService
    {
        public List<LunchBooking> Bookings { get; set; } = new();

        private readonly string _filePath;

        public BookingService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Bookings.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            LoadBookings();
        }

        public List<LunchBooking> GetByUser(string email) =>
            Bookings.Where(p => p.UserEmail == email).ToList();

        public void BookLunch(string userEmail, string date, string option)
        {
            // Remove old booking for same date (only one per day)
            Bookings.RemoveAll(b => b.UserEmail == userEmail && b.Date == date);

            // Add new
            Bookings.Add(new LunchBooking
            {
                Id = Bookings.Any() ? Bookings.Max(b => b.Id) + 1 : 1,
                UserEmail = userEmail,
                Date = date,
                Option = option
            });

            SaveBookings();
        }

        public void DeleteBooking(LunchBooking book)
        {
            Bookings.Remove(book);
            SaveBookings();
        }

        public LunchBooking? GetBooking(string userEmail, string date)
        {
            return Bookings.FirstOrDefault(b => b.UserEmail == userEmail && b.Date == date);
        }

        public List<LunchBooking> GetBookingsByDate(string date)
        {
            return Bookings.Where(b => b.Date == date).ToList();
        }

        private void LoadBookings()
        {
            if (!File.Exists(_filePath))
            {
                Bookings = new List<LunchBooking>();
                return;
            }

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Bookings = new List<LunchBooking>();
                return;
            }

            try
            {
                Bookings = JsonSerializer.Deserialize<List<LunchBooking>>(json) ?? new();
            }
            catch
            {
                Bookings = new List<LunchBooking>();
            }
        }

        private void SaveBookings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Bookings, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
