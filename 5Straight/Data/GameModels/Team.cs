using _5Straight.Data.Models;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Team : TableEntity
    {
        public int TeamNumber { get; set; }

        public string TeamColor { get; set; }

        //Un-serializable: (need to manually rehydrate)
        [IgnoreProperty]
        public List<Player> Players {get; set;}

        public Team()
        {
            Players = new List<Player>();
        }
    }
}
