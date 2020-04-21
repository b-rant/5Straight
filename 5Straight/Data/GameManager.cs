﻿using _5Straight.Data.Models;
using _5Straight.Data.Proxies;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _5Straight.Data
{
    public class GameManager
    {
        public Dictionary<string, Game> Games;
        public GameStateTable GameTable;
        public delegate Task ClientCallback();

        public GameManager(GameStateTable gameStateTable)
        {
            Games = new Dictionary<string, Game>();

            GameTable = gameStateTable;

            // TEMP GAME FOR TESTING!!
            Game game = new Game("Test Game", GameStateFactory.BuildNewGameState(2, 1));
            game.PartitionKey = $"{Games.Count + 1}";

            Games.Add(game.PartitionKey, game);
        }
        public void ConnectClientToGame(string id, ClientCallback callback)
        {
            Games[id].Clients.Add(callback);
        }

        public Game CreateNewGame(string gameName, int numTeams, int numPlayersPerTeam)
        {
            //Game game = GameTable.InsertGame(new Game(gameName, GameStateFactory.BuildNewGameState(numTeams, numPlayersPerTeam)));
            Game game = new Game(gameName, GameStateFactory.BuildNewGameState(numTeams, numPlayersPerTeam));
            game.PartitionKey = $"{Games.Count + 1}";

            Games.Add(game.PartitionKey, game);

            return game;
        }

        public List<Game> GetOpenGames()
        {
            return new List<Game>();
            // comes from Table Service
        }

        public List<Game> GetActiveGamesForUser(string uName)
        {
            return new List<Game>();
            // comes from Table Service
        }

        public bool UserSelectPlayerSlot(string gamePartitionKey, int playerNumber, string userName)
        {
            return Games[gamePartitionKey].OwnPlayerSlot(playerNumber, userName);
        }

        public void AiSelectPlayerSlot(string gamePartitionKey, int playerNumber)
        {
            Games[gamePartitionKey].OwnPlayerSlotForAi(playerNumber);
        }
    }
}
