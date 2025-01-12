namespace DackanTireCenter.Models
{
    public class BookedService
    {
        public string ServiceName { get; set;}
        public decimal Price { get; set;}
        public string CarPlate { get; set;}
        public string CustomerName { get; set;}
        public string Email { get; set;}
        public string Phone { get; set;}
        public DateTime BookingTime { get; set;}

        public BookedService(string serviceName, decimal price, string carPlate, string customerName, string email, string phone, DateTime bookingTime)
        {
            ServiceName = serviceName;
            Price = price;
            CarPlate = carPlate;
            CustomerName = customerName;
            Email = email;
            Phone = phone;
            BookingTime = bookingTime;
        }

        public override string ToString()
        {
            return $"Registreringsnummer: {CarPlate}, {ServiceName} ({Price} SEK) för {CustomerName} på {BookingTime:yyyy-MM-dd HH:mm}";
        }
    }
}