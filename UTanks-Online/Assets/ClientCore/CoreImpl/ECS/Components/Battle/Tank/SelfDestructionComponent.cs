using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using UTanksClient.Core;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;

namespace UTanksClient.ECS.Components.Battle.Tank
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
                ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoDeadId = entity.instanceId, WhoKilledId = entity.instanceId });
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            if(ClientInitService.instance.CheckEntityIsPlayer(entity))
            {
                UIService.instance.ExecuteInstruction((object Obj) =>
                {
                    UIService.instance.battleUIHandler.SelfDestructionWindow.SetActive(true);
                }, null);
            }
            //TimerStart(this.timerAwait, entity, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
            {
                UIService.instance.ExecuteInstruction((object Obj) =>
                {
                    UIService.instance.BattleUI.GetComponent<BattleUIHandler>().SelfDestructionWindow.SetActive(false);
                }, null);
            }
        }
    }
}
