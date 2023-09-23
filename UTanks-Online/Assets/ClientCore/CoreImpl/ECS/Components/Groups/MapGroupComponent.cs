using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(-9076289125000703482)]
    public sealed class MapGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MapGroupComponent() { }
        public MapGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public MapGroupComponent(long Key) : base(Key)
        {
        }
    }
}
