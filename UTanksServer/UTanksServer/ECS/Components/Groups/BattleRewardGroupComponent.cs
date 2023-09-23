using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1513677547945L)]
    public class BattleRewardGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleRewardGroupComponent() { }
        public BattleRewardGroupComponent(ECSEntity entity) : base(entity)
        {
        }

        public BattleRewardGroupComponent(long Key) : base(Key)
        {
        }
    }
}
