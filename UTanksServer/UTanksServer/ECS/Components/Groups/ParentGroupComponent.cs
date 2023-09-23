using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(635908808598551080)]
    public class ParentGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ParentGroupComponent() { }
        public ParentGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public ParentGroupComponent(long Id) : base(Id)
        {
        }
    }
}
