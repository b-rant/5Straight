using _5Straight.Data.Models;
using _5Straight.Data.Proxies;
using System;
using System.Collections.Generic;


namespace _5Straight.Data
{
    public class GameManager
    {
        public Dictionary<string, Game> Games;
        public GameStateTable GameTable;

        public GameManager(GameStateTable gameStateTable)
        {
            Games = new Dictionary<string, Game>();

            GameTable = gameStateTable;
        }

        public Game CreateNewGame(string gameName, int numTeams, int numPlayersPerTeam)
        {
            Game game = GameTable.InsertGame(new Game(gameName, GameStateFactory.BuildNewGameState(numTeams, numPlayersPerTeam)));

            Games.Add(game.PartitionKey, game);

            return game;
        }
    }
}
