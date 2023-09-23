using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(204270486921543070)]
    public class AdminBattleLobbyGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 204270486921543070;
        public AdminBattleLobbyGDAP()
        {
            AvailableComponents = new List<long>() { };
        }
    }
}
