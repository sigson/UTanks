using Core;
using System;
using UTanksServer.Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(-9188485263407476652L)]
    public class SelfDestructionComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        
        public SelfDestructionComponent() { }

        public SelfDestructionComponent(float time)
        {
            this.timerAwait = time;
            onEnd = (entity, selfDestructComp) =>
            {
                try
                {
                    entity.RemoveComponent(selfDestructComp.GetId());
                    ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoDeadId = entity.instanceId, WhoKilledId = entity.instanceId, BattleId = entity.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id).BattleInstanceId });
                }
                catch(Exception ex)
                {
                    Logger.Log("Error self destruction");
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(this.timerAwait, entity, true);
        }
    }
}
