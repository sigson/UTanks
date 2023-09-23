using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Services;

namespace UTanksClient.ECS.Components.Battle.Weapon.Effects
{
    [TypeUid(223323484743283040)]
    public class SelfHealingEffect : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float SelfHealingPercentage;
        public WeaponComponent weaponComponent;

        public SelfHealingEffect(){ }

        public SelfHealingEffect(ECSEntity entity, WeaponComponent damageComponent)//damageComponent
        {
            
        }
    }
}
