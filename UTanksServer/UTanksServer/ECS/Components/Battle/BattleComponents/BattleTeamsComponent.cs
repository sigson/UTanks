using Newtonsoft.Json;
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
    [TypeUid(218165146282063460)]
    public class BattleTeamsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //instance id, compo
        [NonSerialized]
        [JsonIgnore]
        public ConcurrentDictionary<long, TeamComponent> teams = new ConcurrentDictionary<long, TeamComponent>();
    }
}
