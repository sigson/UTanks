using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.Battle.Tank;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.Services;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(229182338035098880)]
    public class DamageEffect : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float SelfHealingPercentage;
        public WeaponComponent weaponComponent;

        public DamageEffect() { }

        public DamageEffect(ECSEntity entity, WeaponComponent damageComponent)//damageComponent
        {
            
        }
    }
}
