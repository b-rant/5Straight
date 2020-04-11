using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Play
    {
        [JsonProperty("playerNum")]
        public int PlayerNumber { get; set; }

        [JsonProperty("turnNum")]
        public int TurnNumber { get; set; }

        [JsonProperty("cardNum")]
        public int CardNumber { get; set; }

        [JsonProperty("draw")]
        public bool Draw { get; set; }

        [JsonProperty("locationNum")]
        public int PlayedLocationNumber { get; set; }

        public List<int> PlayerHand { get; set; }
    }
}