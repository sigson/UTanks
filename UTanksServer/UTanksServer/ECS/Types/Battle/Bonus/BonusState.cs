using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.ECS.Types.Battle.Bonus
{
    public enum BonusState
    {
        Await,
        Dropped,
        Landed,
        Idle,
        Taking,
        Taken,
        Despawned
    }
}
