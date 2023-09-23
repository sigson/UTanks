using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
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

                entity.TryRemoveComponent(timerComp.GetId());
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
        }

        public float ChargingTime { get; set; }
    }
}