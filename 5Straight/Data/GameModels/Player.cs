using _5Straight.Data.GameAI;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class Player : TableEntity
    {
        public int PlayerNumber { get; set; }

        public string PlayerId { get; set; }

        public string PlayerName { get; set; }

        public bool? IsAI { get; set; }

        //Serializable:
        [IgnoreProperty]
        public List<int> Hand { get; set; }

        //Unserializable: (need to manually rehydrate)
        [IgnoreProperty]
        public Team Team { get; set; }

        public AiPlayer Npc { get; set; }

        public Player()
        {
            Hand = new List<int>();
        }

        #region StorageFunctions
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            //write the simple properties:
            var results = base.WriteEntity(operationContext);

            //serialize the complex properties:
            results["Hand"] = new EntityProperty(JsonConvert.SerializeObject(Hand));
            
            if(Npc != null)
            {
                results["IsAI"] = new EntityProperty(true);
            }

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            Hand = JsonConvert.DeserializeObject<List<int>>(properties["Hand"].StringValue);
        }
        #endregion

    }
}