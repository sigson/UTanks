using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.ECSEvents
{
    [TypeUidAttribute(209812350511333540)]
    public class ReceiveEntitiesEvent : ECSEvent
    {
        static public new long Id { get; set; } = 209812350511333540;
        public List<string> Entities;

        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
