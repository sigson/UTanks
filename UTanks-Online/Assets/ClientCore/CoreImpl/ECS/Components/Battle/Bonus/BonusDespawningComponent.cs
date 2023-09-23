using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(185187698180891700)]
    public class BonusDespawningComponent : TimerComponent //deprecated
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusDespawningComponent() { }
        public BonusDespawningComponent(float despawnSecTime)
        {
            timerAwait = despawnSecTime * 1000;
            onEnd = (entity, selfDestructComponent) =>
            {
                
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            
        }
    }
}