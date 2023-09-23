using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.ECS.Events.Battle.TankEvents
{
    [TypeUid(211235083400940300)]
    public class SupplyUsedEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public string supplyPath;
        public List<long> targetEntities = new List<long>();
        public WorldPoint usingPoint;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
