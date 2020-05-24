using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Play : TableEntity
    {
        public int PlayerNumber { get; set; }

        public int TurnNumber { get; set; }

        public int CardNumber { get; set; }

        public bool Draw { get; set; }

        public int PlayedLocationNumber { get; set; }

        //Serializable:
        [IgnoreProperty]
        public List<int> PlayerHand { get; set; }


        #region StorageFunctions
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            //write the simple properties:
            var results = base.WriteEntity(operationContext);

            //serialize the complex properties:
            results["PlayerHand"] = new EntityProperty(JsonConvert.SerializeObject(PlayerHand));

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            PlayerHand = JsonConvert.DeserializeObject<List<int>>(properties["PlayerHand"].StringValue);
        }
        #endregion
    }
}