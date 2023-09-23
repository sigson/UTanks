using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(223782033584306100)]
    public class DamageResistComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WeaponComponent DamageComponent;
        public int WeaponResistPercent { get; set; }
    }
}
