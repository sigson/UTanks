using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.ECS.Templates;
using UTanksServer.ECS.Templates.Battle;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(164487569847138200)]
    public class InBattleChangeEquipmentComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public InBattleChangeEquipmentComponent() { }

        public InBattleChangeEquipmentComponent(float time)
        {
            this.timerAwait = time;
            onEnd = (entity, selfDestructComp) =>
            {
                if(entity.HasComponent(BattleOwnerComponent.Id))
                {
                    entity.RemoveComponent(selfDestructComp.GetId());
                    BattleUserTemplate.GenerateBattleUserEquipment(entity, entity.GetComponent<BattleOwnerComponent>().Battle);
                    new EquipmentTemplate().SetupEntity(entity);
                    ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoDeadId = entity.instanceId, WhoKilledId = entity.instanceId, BattleId = entity.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id).BattleInstanceId });
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(this.timerAwait, entity, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            TimerStop();
        }
    }
}
