using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(205294882888045300)]
    public class TeamPlayerGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 205294882888045300;
        public TeamPlayerGDAP()
        {
            AvailableComponents = new List<long>() { HealthComponent.Id, };
        }
    }
}
