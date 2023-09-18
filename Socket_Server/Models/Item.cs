
namespace Socket_Server.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double? FinalPrice { get; set; }

        public bool Sold { get; set; }
    }
}
