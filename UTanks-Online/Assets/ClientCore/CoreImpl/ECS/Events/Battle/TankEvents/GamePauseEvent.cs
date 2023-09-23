using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents
{
    [TypeUid(228395737493454980)]
    public class GamePauseEvent : ECSEvent //not resurrecting tank
    {
        static public new long Id { get; set; }
        public override void Execute()
        {
            //
        }
    }
}
