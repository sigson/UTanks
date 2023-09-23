using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(643487924561268453)]
    public class DiscountComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DiscountComponent() { }
        public DiscountComponent(float DiscountCoeff)
        {
            this.DiscountCoeff = DiscountCoeff;
        }

        public float DiscountCoeff { get; set; }
    }
}
