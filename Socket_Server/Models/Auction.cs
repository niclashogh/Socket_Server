
namespace Socket_Server.Models
{
    public class Auction
    {
        public Item Item { get; set; }
        public DateTime StartTime { get; set; }
        public int MaxLenght { get; set; }

        //Double = Bet Value
        public IDictionary<double, Participant> Bets { get; set; }
    }
}
