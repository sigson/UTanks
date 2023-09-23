using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.DailyBonus
{
    [TypeUid(238964076762050600)]
    public class DailyBonusCycleComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public int[] Zones { get; set; }
        public DailyBonusData[] DailyBonuses { get; set; }
    }

    [TypeUid(214445102875121600)]
    public class DailyBonusData : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public long Code { get; set; }
        public long CryAmount { get; set; }
        public long XcryAmount { get; set; }
        public long EnergyAmount { get; set; }
        public DailyBonusGarageItemReward ContainerReward { get; set; }
        public DailyBonusGarageItemReward DetailReward { get; set; }
    }

    [TypeUid(223249251913649500)]
    public class DailyBonusGarageItemReward : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public long MarketItemId { get; set; }
        public long Amount { get; set; }
    }
}
