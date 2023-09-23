using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.ECSEvents
{
    [TypeUidAttribute(209812350511333540)]
    public class ReceiveEntitiesEvent : ECSEvent
    {
        static public new long Id { get; set; } = 217578455225018140;
        public List<string> Entities;

        public override void Execute()
        {
            foreach (var entity in Entities)
            {
                //entity.entityComponents.RestoreComponentsAfterSerialization(entity);
                EntitySerialization.UpdateDeserialize(entity);
            }
        }
    }
}
