using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Garage
{
    [TypeUid(215086673547113400)]
    public class DiscountListComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public Dictionary<string, float> Discounts { get; set; } = new Dictionary<string, float>();

        public DiscountListComponent() { }
        public DiscountListComponent(Dictionary<string, float> discounts) { Discounts = discounts; }
    }
}
