using UTanksClient.Core;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.DailyBonus
{
    [TypeUid(636459062089487176)]
    public class UserDailyBonusZoneComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public UserDailyBonusZoneComponent(Player player)
        //{
        //    ZoneNumber = player.Data.DailyBonusZone;
        //    SelfOnlyPlayer = player;
        //}

        public long ZoneNumber { get; set; }
    }
}
