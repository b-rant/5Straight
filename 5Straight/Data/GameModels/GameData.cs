using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class GameData
    {
        public GameData(List<BoardLocation> board, List<int> deck, List<Player> players)
        {
            Board = board;
            Deck = deck;
            Players = players;
            Plays = new List<Play>();
            TurnNumber = 0;
            Won = false;
            HighestPlayable = 99;
        }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("won")]
        public bool Won { get; set; }

        [JsonProperty("turnNumber")]
        public int TurnNumber { get; set; }

        [JsonProperty("highestPlayable")]
        public int HighestPlayable { get; set; }

        [JsonProperty("board")]
        public List<BoardLocation> Board { get; set; }

        [JsonProperty("deck")]
        public List<int> Deck { get; set; }

        [JsonProperty("plays")]
        public List<Play> Plays { get; set; }

        [JsonProperty("players")]
        public List<Player> Players { get; set; }

        [JsonProperty("CurrentPlayer")]
        public Player CurrentPlayer { get; set; }

        [JsonProperty("WinningPlayer")]
        public Player WinningPlayer { get; internal set; }
    }
}
