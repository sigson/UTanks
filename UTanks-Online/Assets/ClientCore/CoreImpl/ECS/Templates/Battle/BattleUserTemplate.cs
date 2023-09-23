using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.Battle.Location;
using UTanksClient.ECS.Components.Battle.Round;
using UTanksClient.ECS.Components.Battle.Tank;
using UTanksClient.ECS.Components.Battle.Team;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Templates.Battle
{
    [TypeUid(183156117890446200)]
    public class BattleUserTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 183156117890446200;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }

        public override void InitializeConfigsPath()
        {
            
        }
    }
}
