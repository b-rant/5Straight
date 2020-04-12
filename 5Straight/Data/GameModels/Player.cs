using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Player
    {

        [JsonProperty("playerNum")]
        public int PlayerNumber { get; set; }

        [JsonProperty("hand")]
        public List<int> Hand { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        public Team Team { get; set; }

        public string PlayerOwner { get; set; }

    }
}