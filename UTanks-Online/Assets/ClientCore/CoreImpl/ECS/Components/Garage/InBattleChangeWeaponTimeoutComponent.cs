using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Garage
{
    [TypeUid(242564821758049900)]
    public class InBattleChangeWeaponTimeoutComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public InBattleChangeWeaponTimeoutComponent()
        {
            onEnd = (entity, timerComponent) => {
                this.ownerEntity.RemoveComponent(this.GetId());
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            //this.TimerStart(180f, null, true);
        }
    }
}
