using Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(7115193786389139467)]
    public class WeaponCooldownComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponCooldownComponent() { }
        public WeaponCooldownComponent(float cooldownIntervalSec)
        {
            CooldownIntervalSec = cooldownIntervalSec;
            onEnd = (entity, timerComponent) => {
                var timerComp = timerComponent as WeaponCooldownComponent;

                entity.TryRemoveComponent(timerComp.GetId());
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