using System.Collections.Generic;
using System.Linq;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1507791699587)]
    public class CountableItemsPackComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public CountableItemsPackComponent() { }
        public CountableItemsPackComponent(Dictionary<ECSEntity, int> pack)
        {
            Pack = pack.ToDictionary(x => x.Key.GetId(), x => x.Value);
        }

        public Dictionary<long, int> Pack { get; set; }
    }
}
