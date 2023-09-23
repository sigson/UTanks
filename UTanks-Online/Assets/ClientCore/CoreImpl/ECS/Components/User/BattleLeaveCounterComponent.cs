using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(1502092676956)]
    public class BattleLeaveCounterComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleLeaveCounterComponent() { }
        public BattleLeaveCounterComponent(long value, int needGoodBattles)
        {
            Value = value;
            NeedGoodBattles = needGoodBattles;
        }

        public long Value { get; set; }
        public int NeedGoodBattles { get; set; }

        [ProtocolIgnore] public int GoodBattlesInRow { get; set; }
    }
}
