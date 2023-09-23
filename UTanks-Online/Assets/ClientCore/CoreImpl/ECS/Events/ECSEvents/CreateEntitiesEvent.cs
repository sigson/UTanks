using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.ECSEvents
{
    [TypeUidAttribute(217578455225018140)]
    public class RemoveEntitiesEvent : ECSEvent //obsolete
    {
        static public new long Id { get; set; } = 217578455225018140;
        public long EntityIdRecipient; //ID of user with socket component
        public List<long> Entities;
        public override void Execute()
        {
            foreach (var entity in Entities)
            {
                //entity.entityComponents.RestoreComponentsAfterSerialization(entity);
                //ManagerScope.entityManager.OnRemoveEntity(ManagerScope.entityManager.EntityStorage[entity]);
            }
        }
    }
}
