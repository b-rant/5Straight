using _5Straight.Data.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5Straight.Data.Proxies
{
    public class GameStateTable
    {
        private TableService tableService;
        private string tableName = "GameStates";

        public GameStateTable(TableService tableService)
        {
            this.tableService = tableService;
        }

        #region InsertOrUpdate methods
        public Game InsertGame(Game game)
        {
            if (game is null)
            {
                throw new ArgumentNullException("game");
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(game);

                TableResult response = tableService.Execute(insertOrMergeOperation, tableName);

                InsertGameState(game.GameState);

                return response.Result as Game;
            }
            catch (StorageException e)
            {
                throw;
            }
        }

        public async void InsertGameState(GameState gameState)
        {
            if (gameState is null)
            {
                throw new ArgumentNullException("gameState");
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(gameState);

                TableResult result = tableService.Execute(insertOrMergeOperation, tableName);
            }
            catch (StorageException e)
            {
                throw;
            }
        }
        #endregion

        #region Retrieval methods
        public GameState GetGameState(string partitionKey, string rowKey)
        {
            if (partitionKey is null)
            {
                throw new ArgumentNullException("partitionKey");
            }
            else if (rowKey is null)
            {
                throw new ArgumentNullException("rowKey");
            }

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);

                var response = tableService.Execute(retrieveOperation, tableName);

                return (GameState) response.Result;
            }
            catch (StorageException e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<GameState>> GetGameStatesForUser(string userNameOrSomething)
        {
            try
            {
                TableQuery<GameState> query = new TableQuery<GameState>();

                var gameStates = await Task.Run(() => tableService.ExecuteQuery(query, tableName));

                return gameStates;
            }
            catch (StorageException e)
            {
                throw;
            }
        }
        #endregion
    }
}
