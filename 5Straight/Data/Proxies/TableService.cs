using Blazorise;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5Straight.Data.Proxies
{
    public class TableService
    {
        private CloudTableClient tableClient;

        public TableService()
        {
            AzureServiceTokenProvider astp = new AzureServiceTokenProvider();

            KeyVaultClient kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(astp.KeyVaultTokenCallback));

            string storageAccessKey = kvc.GetSecretAsync("https://kv-5straight.vault.azure.net/secrets/storageaccesskey").Result.Value;

            tableClient = new CloudTableClient(
                new Uri("https://5straight.table.core.windows.net/"),
                new StorageCredentials("5straight", storageAccessKey));

        }

        public TableResult Execute(TableOperation tableOp, string tableName)
        {
            try
            {
                CloudTable table = tableClient.GetTableReference(tableName);

                TableResult result = table.Execute(tableOp);

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public TableBatchResult ExecuteBatch(TableBatchOperation tableOp, string tableName)
        {
            if (tableOp.Count > 0) //Cannot execute an empty batch
            {
                try
                {
                    CloudTable table = tableClient.GetTableReference(tableName);

                    TableBatchResult result = table.ExecuteBatch(tableOp);

                    return result;
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            return new TableBatchResult();
        }

        public IEnumerable<t> ExecuteQuery<t>(TableQuery<t> query, string tableName) where t : TableEntity, new()
        {
            try
            {
                CloudTable table = tableClient.GetTableReference(tableName);

                var results = table.ExecuteQuery<t>(query);

                return results;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
