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
        public GameTable GameTable;
        public delegate Task ClientCallback();
        public readonly List<Delegate> Clients;

        public GameManager(GameTable gameTable)
        {
            Clients = new List<Delegate>();
            GameTable = gameTable;
            Games = GameTable.LoadAllGames();

            foreach(Game g in Games.Values)
            {
                g.SaveGameDelegate = SaveGame;
            }
        }

        public void SaveGame(Game game)
        {
            GameTable.SaveGame(game);
        }

        public void ConnectClientToGame(string id, ClientCallback callback)
        {
            if(!Games[id].Clients.Contains(callback))
            {
                Games[id].Clients.Add(callback);
            }
        }

        public void ConnectClientToGameManager(ClientCallback callback)
        {
            if (!Clients.Contains(callback))
            {
                Clients.Add(callback);
            }
        }

        public void DisconnectClientFromGame(string id, ClientCallback callback)
        {
            if (Games[id].Clients.Contains(callback))
            {
                Games[id].Clients.Remove(callback);
            }
        }
        
        public void DisconnectClientFromGameManager(ClientCallback callback)
        {
            if (Clients.Contains(callback))
            {
                Clients.Remove(callback);
            }
        }

        private async void UpdateEveryone()
        {
            foreach (Delegate d in Clients)
            {
                await Task.Run(() => d.DynamicInvoke());
            }
        }

        public Game CreateNewGame(string gameName, int numTeams, int numPlayersPerTeam)
        {
            //Game game = GameTable.InsertGame(new Game(gameName, GameStateFactory.BuildNewGameState(numTeams, numPlayersPerTeam)));
            Game game = GameFactory.BuildNewGame(gameName, numTeams, numPlayersPerTeam);
            //game.PartitionKey = $"{Games.Count + 1}";

            game.SaveGameDelegate = SaveGame;
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

        #region Gameplay function passthrough

        public bool UserSelectPlayerSlot(string gamePartitionKey, int playerNumber, string userId, string userGivenName)
        {
            try
            {
                var success = Games[gamePartitionKey].OwnPlayerSlot(playerNumber, userId, userGivenName);
                UpdateEveryone();
                if (Games[gamePartitionKey].ValidateAndStartGame())
                {
                    RunGameAi(gamePartitionKey);
                }
                return success;
            }
            catch
            {
                return false;
            }
        }

        public bool UserDeSelectPlayerSlot(string gamePartitionKey, int playerNumber)
        {
            try
            {
                var success = Games[gamePartitionKey].RemovePlayerSlot(playerNumber);
                UpdateEveryone();
                return success;
            }
            catch
            {
                return false;
            }
        }

        public void AiSelectPlayerSlot(string gamePartitionKey, int playerNumber)
        {
            Games[gamePartitionKey].OwnPlayerSlotForAi(playerNumber);
            UpdateEveryone();
            if (Games[gamePartitionKey].ValidateAndStartGame())
            {
                RunGameAi(gamePartitionKey);
            }
        }

        public void AiDeselectPlayerSlot(string gamePartitionKey, int playerNumber)
        {
            Games[gamePartitionKey].RemoveAiFromPlayerSlot(playerNumber);
            UpdateEveryone();
        }

        public string UserMakePlay(string gamePartitionKey, Player player, bool draw, int location = -1, int card = -1)
        {
            string response;
            if (draw)
            {
                response = Games[gamePartitionKey].PlayDrawCard(player);
            }
            else
            {
                response = Games[gamePartitionKey].PlayLocation(player, location, card);
            }

            UpdateEveryone();
            RunGameAi(gamePartitionKey);
            return response;
        }

        private async void RunGameAi(string gamePartitionKey)
        {
            while(!Games[gamePartitionKey].Won && Games[gamePartitionKey].CurrentPlayer.Npc != null)
            {
                await Games[gamePartitionKey].RunAI();
                UpdateEveryone();
            }
        }

        #endregion
    }
}
