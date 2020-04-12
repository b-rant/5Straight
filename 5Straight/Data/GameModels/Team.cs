using _5Straight.Data.Models;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Team
    {
        public int TeamNumber { get; set; }

        public string TeamColor { get; set; }

        public List<Player> Players {get; set;}
    }
}
