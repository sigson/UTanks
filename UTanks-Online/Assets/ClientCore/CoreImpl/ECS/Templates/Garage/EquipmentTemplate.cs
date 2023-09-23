using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Chassis;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ComponentsGroup.Garage;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.Services;

namespace UTanksClient.ECS.Templates
{
    [TypeUid(225826000761710720)]
    public class EquipmentTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 225826000761710720;

        public override void InitializeConfigsPath()
        {
            
        }

        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }
    }
}
