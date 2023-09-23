using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle
{
    [TypeUid(120924393017466430)]
    public class SpectatorSwitchCooldownComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public List<object> techObjects = new List<object>();

        public SpectatorSwitchCooldownComponent() { }
        public SpectatorSwitchCooldownComponent(float cooldownIntervalSec)
        {
            CooldownIntervalSec = cooldownIntervalSec;
            onEnd = (entity, timerComponent) => {
                var timerComp = timerComponent as SpectatorSwitchCooldownComponent;

                entity.RemoveComponent(timerComp.GetId());
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
