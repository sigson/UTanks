using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.DailyBonus
{
    [TypeUid(636462569803130386L)]
    public class DailyBonusCommonConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public long ReceivingBonusIntervalInSeconds { get; set; }
        public long BattleCountToUnlockDailyBonuses { get; set; }
        public int PremiumTimeSpeedUp { get; set; }
    }
}
