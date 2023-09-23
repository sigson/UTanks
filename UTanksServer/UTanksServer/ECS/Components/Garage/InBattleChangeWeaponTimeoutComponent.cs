using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Garage
{
    [TypeUid(242564821758049900)]
    public class InBattleChangeWeaponTimeoutComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public InBattleChangeWeaponTimeoutComponent()
        {
            onEnd = (entity, timerComponent) => {
                this.ownerEntity.TryRemoveComponent(this.GetId());
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(180f, null, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            TimerStop();
            onEnd(entity, this);
        }
    }
}
