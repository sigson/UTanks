using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(234587352361277540)]
    public class BattleMemberGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 234587352361277540;
        public BattleMemberGDAP()
        {
            RestrictedComponents = new List<long>() {
                BattleSimpleInfoComponent.Id,
                BattleComponent.Id,
                DMComponent.Id,
                TDMComponent.Id,
                CTFComponent.Id,
                DOMComponent.Id,
                RoundRestartingStateComponent.Id,
                RoundFundComponent.Id,
                BattleCreatureStorageComponent.Id,
                BattleScoreComponent.Id,
                BattleDropStorageComponent.Id
            };
        }
    }
}
