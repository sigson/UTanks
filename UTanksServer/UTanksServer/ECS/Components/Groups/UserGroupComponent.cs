using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(7453043498913563889)]
    public sealed class UserGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserGroupComponent() { }
        public UserGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public UserGroupComponent(long Key) : base(Key)
        {
        }
    }
}
