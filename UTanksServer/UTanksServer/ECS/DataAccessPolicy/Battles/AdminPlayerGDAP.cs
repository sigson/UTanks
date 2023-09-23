using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(214399685934859800)]
    public class AdminPlayerGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 214399685934859800;
        public AdminPlayerGDAP()
        {
            AvailableComponents = new List<long>() { };
        }
    }
}
