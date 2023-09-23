using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(226719446878951940)]
    public class DropBonuseGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 226719446878951940;
        public DropBonuseGDAP()
        {
            AvailableComponents = new List<long>() {BonusComponent.Id};
        }
    }
}
