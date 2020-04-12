using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class GameState
    {
        public GameState(List<BoardLocation> board, List<int> deck, List<Team> teams, List<Player> players)
        {
            Board = board;
            Deck = deck;
            Teams = teams;
            Players = players;
            CurrentPlayer = players[0];
            Plays = new List<Play>();
            TurnNumber = 0;
            Won = false;
            HighestPlayable = 99;
        }

        public string Id { get; set; }

        public bool Won { get; set; }

        public int TurnNumber { get; set; }

        public int HighestPlayable { get; set; }

        public List<BoardLocation> Board { get; set; }

        public List<int> Deck { get; set; }

        public List<Play> Plays { get; set; }

        public List<Team> Teams { get; set; }

        public List<Player> Players { get; set; }

        public Player CurrentPlayer { get; set; }

        public Player WinningPlayer { get; internal set; }
    }
}
