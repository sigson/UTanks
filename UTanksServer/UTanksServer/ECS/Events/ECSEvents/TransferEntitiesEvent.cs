using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.ECSEvents
{
    [TypeUidAttribute(212300185271845500)]
    public class TransferEntitiesEvent : ECSEvent
    {
        static public new long Id { get; set; } = 212300185271845500;
        public long EntityIdRecipient; //ID of user with socket component
        public List<string> Entities;
        public override void Execute()
        {
            foreach (var entity in Entities)
            {
                //entity.entityComponents.RestoreComponentsAfterSerialization(entity);
                //ManagerScope.entityManager.OnAddNewEntity(EntitySerialization.Deserialize(entity));
            }
        }
    }
}
