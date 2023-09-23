using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.BattleComponents
{
    [TypeUid(188254263019633600)]
    public class BattlePlayersComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public ConcurrentDictionary<ECSEntity, TeamComponent> players = new ConcurrentDictionary<ECSEntity, TeamComponent>();
    }
}
