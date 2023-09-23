using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
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
