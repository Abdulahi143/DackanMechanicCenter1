namespace DackanTireCenter.Models
{
    public class Service
    {
        public string Name { get; }
        public decimal Price { get; }

        public Service(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }

}
