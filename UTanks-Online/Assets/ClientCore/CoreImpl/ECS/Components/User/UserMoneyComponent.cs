using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(9171752353079252620L)]
    public class UserMoneyComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserMoneyComponent() { }
        public UserMoneyComponent(long Money)
        {
            this.Money = Money;
        }

        public long Money { get; set; }
    }
}
