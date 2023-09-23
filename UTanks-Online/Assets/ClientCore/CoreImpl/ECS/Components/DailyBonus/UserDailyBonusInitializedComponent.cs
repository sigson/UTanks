using UTanksClient.Core;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.DailyBonus
{
    [TypeUid(636459225080719742)]
    public class UserDailyBonusInitializedComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public UserDailyBonusInitializedComponent(Player selfOnlyPlayer) => SelfOnlyPlayer = selfOnlyPlayer;
    }
}
