using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Round
{
    //storing exited players statistic for future reinvite and crystal payout
    [TypeUid(1958706908788302656)]
    public class RoundUsersStatisticsStorageComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public ConcurrentDictionary<ECSEntity, RoundUserStatisticsComponent> roundUserStatisticsComponents = new ConcurrentDictionary<ECSEntity, RoundUserStatisticsComponent>();
    }
}
