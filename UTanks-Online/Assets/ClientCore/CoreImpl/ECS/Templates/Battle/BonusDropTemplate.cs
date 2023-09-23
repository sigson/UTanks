using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.ECS.Components.ECSComponents;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace UTanksClient.ECS.Templates.Battle
{
    [TypeUid(209316286945445700)]
    public class BonusDropTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }

        public override void InitializeConfigsPath()
        {
            
        }
    }
}
