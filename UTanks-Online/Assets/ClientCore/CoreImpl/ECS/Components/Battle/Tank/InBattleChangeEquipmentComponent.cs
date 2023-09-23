using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle.Tank
{
    [TypeUid(164487569847138200)]
    public class InBattleChangeEquipmentComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public InBattleChangeEquipmentComponent() { }

        public InBattleChangeEquipmentComponent(float time)
        {
            //this.timerAwait = time;
            //onEnd = (entity, selfDestructComp) =>
            //{
            //    ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoDeadId = entity.instanceId, WhoKilledId = entity.instanceId });
            //};
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            if (ClientInitService.instance.CheckEntityIsPlayer(entity))
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
                    UIService.instance.battleUIHandler.SelfDestructionWindow.SetActive(false);
                }, null);
            }
        }
    }
}
