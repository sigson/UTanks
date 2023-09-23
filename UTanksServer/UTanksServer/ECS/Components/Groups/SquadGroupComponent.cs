using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Groups
{
    [TypeUid(1507120787784L)]
    public sealed class SquadGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SquadGroupComponent() { }
        public SquadGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public SquadGroupComponent(long Key) : base(Key)
        {
        }
    }
}