using System;
using UTanksServer.Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.DailyBonus
{
    [TypeUid(636462622709176439L)]
	public class UserDailyBonusLastReceivingDateComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserDailyBonusLastReceivingDateComponent(long date)
        {
            Date = date;
        }
        public UserDailyBonusLastReceivingDateComponent() { }

        [OptionalMapped] public long Date { get; set; } = DateTime.UtcNow.Ticks;
        public long TotalMillisLength { get; set; } = 24000000;
	}
}
