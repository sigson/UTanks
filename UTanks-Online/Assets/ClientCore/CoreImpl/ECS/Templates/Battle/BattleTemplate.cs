using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.Battle.Round;
using UTanksClient.ECS.Components.Battle.Team;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.ECS.Types.Lobby;

namespace UTanksClient.ECS.Templates.Battle
{
    [TypeUid(216086468345602700)]
    public class BattleTemplate : EntityTemplate
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
