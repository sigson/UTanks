using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(197120840123223600)]
    public class HammerDamageComponent : WeaponComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            return null;
        }
    }
}
