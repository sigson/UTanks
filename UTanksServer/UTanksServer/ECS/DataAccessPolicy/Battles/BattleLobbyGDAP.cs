using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(211895083904190530)]
    public class BattleLobbyGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 211895083904190530;
        public BattleLobbyGDAP()
        {
            AvailableComponents = new List<long>() { 
                BattleSimpleInfoComponent.Id,
                BattleComponent.Id,
                DMComponent.Id,
                TDMComponent.Id,
                CTFComponent.Id,
                DOMComponent.Id
            };
        }
    }
}
