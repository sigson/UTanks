using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1479807693001L)]
    public class UserItemCounterComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserItemCounterComponent() { }
        public UserItemCounterComponent(long count)
        {
            Count = count;
        }

        public long Count { get; set; }
    }
}
