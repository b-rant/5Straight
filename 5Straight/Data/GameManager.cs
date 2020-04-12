using _5Straight.Data.Models;
using System;
using System.Collections.Generic;


namespace _5Straight.Data
{
    public class GameManager
    {
        public Dictionary<string, GameState> Games;

        public GameManager()
        {
            Games = new Dictionary<string, GameState>
            {
                { "1", GameStateFactory.BuildNewGameState(3,2) }
            };
        }
    }
}
