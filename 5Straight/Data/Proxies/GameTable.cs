using _5Straight.Data.GameAI;
using _5Straight.Data.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5Straight.Data.Proxies
{
    public class GameTable
    {
        private TableService tableService;

        public GameTable(TableService tableService)
        {
            this.tableService = tableService;
        }

        #region Write methods
        public void SaveGame(Game game)
        {
            if (game is null)
            {
                throw new ArgumentNullException("game");
            }

            try
            {
                //Game
                TableOperation saveGame = TableOperation.InsertOrReplace(game);

                TableResult saveGameResult = tableService.Execute(saveGame, "Games");


                //Board (or BoardLocations)
                TableBatchOperation saveBoard = new TableBatchOperation();

                foreach (BoardLocation bl in game.Board)
                {
                    bl.PartitionKey = game.PartitionKey;
                    bl.RowKey = bl.DisplayNumber;
                    saveBoard.InsertOrReplace(bl);
                }

                TableBatchResult saveBoardResult = tableService.ExecuteBatch(saveBoard, "BoardLocations");


                //Plays
                TableBatchOperation savePlays = new TableBatchOperation();

                foreach (Play p in game.Plays)
                {
                    p.PartitionKey = game.PartitionKey;
                    p.RowKey = p.TurnNumber.ToString();
                    savePlays.InsertOrReplace(p);
                }

                TableBatchResult savePlaysResult = tableService.ExecuteBatch(savePlays, "Plays");


                //Teams
                TableBatchOperation saveTeams = new TableBatchOperation();

                foreach (Team t in game.Teams)
                {
                    t.PartitionKey = game.PartitionKey;
                    t.RowKey = t.TeamNumber.ToString();
                    saveTeams.InsertOrReplace(t);
                }

                TableBatchResult saveTeamsResult = tableService.ExecuteBatch(saveTeams, "Teams");


                //Players
                TableBatchOperation savePlayers = new TableBatchOperation();

                foreach (Player p in game.Players)
                {
                    p.PartitionKey = game.PartitionKey;
                    p.RowKey = p.PlayerNumber.ToString();
                    savePlayers.InsertOrReplace(p);
                }

                TableBatchResult savePlayersResult = tableService.ExecuteBatch(savePlayers, "Players");
            }
            catch (StorageException e)
            {
                throw;
            }
        }
        #endregion

        #region Read methods
        public Dictionary<string, Game> LoadAllGames()
        {
            Dictionary<string, Game> allGames = new Dictionary<string, Game>();

            try
            {
                TableQuery<Game> loadGames = new TableQuery<Game>().Select(new List<string>()); //only get partitionkey and rowkey
                var loadGamesResult = tableService.ExecuteQuery(loadGames, "Games");
                
                foreach(Game g in loadGamesResult)
                {
                    Game g2 = LoadGame(g.PartitionKey);
                    allGames.Add(g2.PartitionKey, g2);
                }
            }
            catch (StorageException e)
            {
                throw;
            }

            return allGames;
        }

        public Game LoadGame(string partitionKey)
        {
            try
            {
                //Game
                TableOperation loadGame = TableOperation.Retrieve<Game>(partitionKey, "Game");
                TableResult loadGameResult = tableService.Execute(loadGame, "Games");
                Game game = loadGameResult.Result as Game;

                //Players
                TableQuery<Player> loadPlayers = new TableQuery<Player>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
                var players = tableService.ExecuteQuery(loadPlayers, "Players");
                game.Players = players.ToList<Player>();

                //Teams
                TableQuery<Team> loadTeams = new TableQuery<Team>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
                var teams = tableService.ExecuteQuery(loadTeams, "Teams");
                game.Teams = teams.ToList<Team>();

                //Wire-up references between Teams and Players:
                foreach (Player p in game.Players)
                {
                    p.Team = game.Teams[p.PlayerNumber % game.Teams.Count];
                    game.Teams[p.PlayerNumber % game.Teams.Count].Players.Add(p);

                    if(p.IsAI.Equals(true))
                    {
                        p.Npc = AiPlayerFactory.BuildAi(p, game);
                    }
                }

                //Board (aka BoardLocations)
                TableQuery<BoardLocation> loadBoard = new TableQuery<BoardLocation>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
                var board = tableService.ExecuteQuery(loadBoard, "BoardLocations");
                game.Board = board.ToList<BoardLocation>();

                //Wire-up references within BoardLocation:
                foreach (BoardLocation bl in game.Board)
                {
                    if (bl.FilledByPlayerNumber >= 0)
                    {
                        bl.FilledBy = game.Players[bl.FilledByPlayerNumber];
                    }
                }

                //Plays
                TableQuery<Play> loadPlays = new TableQuery<Play>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
                var plays = tableService.ExecuteQuery(loadPlays, "Plays");
                game.Plays = plays.ToList<Play>();
                //Play has no references to rehydrate.

                //Current Player
                game.CurrentPlayer = game.Players[game.TurnNumber % game.Players.Count];

                //Winning Player
                if (game.Won == true)
                {
                    game.WinningPlayer =  game.Players[game.Plays.OrderByDescending(x => x.TurnNumber).ToList().Last().PlayerNumber];
                }

                game.RunAI();

                return game;
            }
            catch (StorageException e)
            {
                throw;
            }
        }
        #endregion
    }
}
