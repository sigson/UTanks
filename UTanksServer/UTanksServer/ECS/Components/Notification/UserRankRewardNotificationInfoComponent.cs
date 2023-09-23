using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(636147227222284613L)]
    public class UserRankRewardNotificationInfoComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserRankRewardNotificationInfoComponent() { }
        public UserRankRewardNotificationInfoComponent(long xCrystals, long crystals, long rank)
        {
            RedCrystals = xCrystals;
            BlueCrystals = crystals;
            Rank = rank;
        }

        public long RedCrystals { get; set; }
        public long BlueCrystals { get; set; }
        public long Rank { get; set; }
    }
}
