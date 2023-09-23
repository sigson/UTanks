using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(191480316920133020)]
    public class DMPlayerGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 191480316920133020;
        public DMPlayerGDAP()
        {
            AvailableComponents = new List<long>() { };
        }
    }
}
