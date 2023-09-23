using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(636177019617146882)]
    public class SpecialOfferDurationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public bool OneShot { get; set; } = false;
    }
}
