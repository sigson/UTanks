using SecuredSpace.ClientControl.Model;
using SecuredSpace.Battle.Tank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Components.Battle;
using static UTanksClient.ECS.ECSCore.ComponentsDBComponent;

namespace SecuredSpace.Battle.Creatures
{
    public abstract class ICreature : IComponentManager
    {
        public long CreatureInstanceId;
        public long CreatureOwnerId;
        [HideInInspector]
        public TankManager ownerTankManager;
        public long DBOwner;

        public static ICreature Create(ICreatureComponent creatureComponent)
        {
            return null;
        }

        public virtual void UpdateCreatureState(ICreatureComponent creatureComponent, ComponentState state)
        {

        }
    }
}