using SecuredSpace.Battle.Tank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle
{
    [TypeUid(120236439017466430)]
    public class RebuildAfterDeathComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public RebuildAfterDeathComponent() { }
        public RebuildAfterDeathComponent(float cooldownIntervalSec)
        {
            CooldownIntervalSec = cooldownIntervalSec;
            onEnd = (entity, timerComponent) => {
                var timerComp = timerComponent as RebuildAfterDeathComponent;
                try
                {
                    if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var manager))
                    {
                        manager.ExecuteInstruction(() => {
                            manager.RebuildTank(entity);
                            //manager.EnableGhostTankState();
                        });
                        //bmark: zalupa
                        //if (entity.isPlayer && false == true)
                        //    ClientInitService.instance.battleManager.BattleSupplyUIPrepare(entity, ManagerScope.entityManager.EntityStorage[entity.GetComponent<BattleOwnerComponent>().BattleInstanceId].GetComponent<BattleComponent>());
                    }
                    if (entity.HasComponent(timerComp.GetId()))
                        entity.RemoveComponent(timerComp.GetId());
                }
                catch { }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(CooldownIntervalSec, entity, true);
        }

        public float CooldownIntervalSec { get; set; }
    }
}
