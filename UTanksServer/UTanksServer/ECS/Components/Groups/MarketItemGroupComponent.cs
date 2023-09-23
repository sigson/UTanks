using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(63290793489633843)]
    public sealed class MarketItemGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MarketItemGroupComponent() { }
        public MarketItemGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public MarketItemGroupComponent(long Key) : base(Key)
        {
        }
    }
}
