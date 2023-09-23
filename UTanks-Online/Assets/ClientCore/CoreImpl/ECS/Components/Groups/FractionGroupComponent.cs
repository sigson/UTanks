using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1544510801819)]
    public class FractionGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public FractionGroupComponent() { }
        public FractionGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public FractionGroupComponent(long Id) : base(Id)
        {
        }
    }
}
