using UTanksServer.Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.DailyBonus
{
    [TypeUid(636459034861529826)]
    public class UserDailyBonusCycleComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public UserDailyBonusCycleComponent(Player player)
        //{
        //    CycleNumber = player.Data.DailyBonusCycle;
        //    SelfOnlyPlayer = player;
        //}

        public long CycleNumber { get; set; }
    }
}
