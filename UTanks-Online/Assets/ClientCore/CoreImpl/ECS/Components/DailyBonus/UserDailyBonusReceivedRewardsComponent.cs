using System.Collections.Generic;
using UTanksClient.Core;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.DailyBonus
{
    [TypeUid(636459174909060087)]
    public class UserDailyBonusReceivedRewardsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public UserDailyBonusReceivedRewardsComponent(Player player)
        //{
        //    ReceivedRewards = player.Data.DailyBonusReceivedRewards;
        //    SelfOnlyPlayer = player;
        //}

        public List<long> ReceivedRewards { get; set; }
    }
}
