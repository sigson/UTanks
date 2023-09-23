using SecuredSpace.Battle.Tank;
using SecuredSpace.Battle.Tank.Turret;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(2654416098660377118L)]
    public class RailgunChargingWeaponComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public RailgunChargingWeaponComponent() { }
        public RailgunChargingWeaponComponent(float chargingTime)
        {
            ChargingTime = chargingTime;
            onEnd = (entity, timerComponent) => {
                var timerComp = timerComponent as RailgunChargingWeaponComponent;

                entity.RemoveComponent(timerComp.GetId());
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(ChargingTime, entity, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    (tankManager.turretManager as RailgunManager).Shot();
                }, "Error railgun shot");
            }
        }

        public float ChargingTime { get; set; }
    }
}