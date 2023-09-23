using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(188632253995440770)]
    public class SpectatorGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 188632253995440770;
        public SpectatorGDAP()
        {
            AvailableComponents = new List<long>() {};
        }
    }
}
