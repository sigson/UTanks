using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Garage
{
    [TypeUid(229658540996011970)]
    public class UpdateSupplyCountEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public List<Types.Supply> InBattleSupply = new List<Types.Supply>();
        public List<Types.Supply> SyncSupply = new List<Types.Supply>();
        //public Dictionary<string, int> GarageSupply = new Dictionary<string, int>();
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
