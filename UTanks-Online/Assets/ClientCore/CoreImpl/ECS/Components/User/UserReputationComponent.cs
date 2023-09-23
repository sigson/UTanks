using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1502716170372)]
    public class UserReputationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserReputationComponent() { }
        public UserReputationComponent(double Reputation)
        {
            this.Reputation = Reputation;
        }

        public double Reputation { get; set; }
    }
}
