using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
	[TypeUid(213285507406410430)]
	public class RicochetDamageComponent : WeaponComponent
	{
		static public new long Id { get; set; }
		static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            return null;
        }
    }
}
