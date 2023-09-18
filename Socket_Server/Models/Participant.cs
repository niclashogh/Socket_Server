
namespace Socket_Server.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Item> Collection { get; set; }
    }
}
