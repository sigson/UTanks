using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.ECSEvents
{
    [TypeUidAttribute(230864384312783680)]
    public class UpdateEntitiesEvent : ECSEvent
    {
        static public new long Id { get; set; } = 230864384312783680;
        public long EntityIdRecipient; //ID of user with socket component
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
