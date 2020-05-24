using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class BoardLocation : TableEntity
    {
        public BoardLocation()
        {
            FilledBy = null;
            FilledByPlayerNumber = -1;
        }

        public int Number { get; set; }

        public string DisplayNumber { get; set; }

        public bool Filled { get; set; }

        public int FilledByPlayerNumber { get; set; }

        //Serializable properties:
        [IgnoreProperty]
        public List<int?> AdjacentLocations { get; set; }

        //Un-serializable: (need to manually rehydrate)
        [IgnoreProperty]
        public Player FilledBy { get; set; }

        #region StorageFunctions
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            //write the simple properties:
            var results = base.WriteEntity(operationContext);

            //serialize the complex properties:
            results["AdjacentLocations"] = new EntityProperty(JsonConvert.SerializeObject(AdjacentLocations));

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            AdjacentLocations = JsonConvert.DeserializeObject<List<int?>>(properties["AdjacentLocations"].StringValue);
        }
        #endregion
    }
}
