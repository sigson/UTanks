using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Offer
{
    [TypeUid(636179208446312959L)]
    public class SpecialOfferRemainingTimeComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SpecialOfferRemainingTimeComponent() { }
        public SpecialOfferRemainingTimeComponent(long remain = 86399996400)
        {
            Remain = remain;
        }

        public long Remain { get; set; }
    }
}
