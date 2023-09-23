using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Item
{
    [TypeUid(636457322095664962L)]
    public class DetailItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public long TargetMarketItemId { get; set; }
        public long RequiredCount { get; set; }
    }
}
