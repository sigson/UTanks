using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(1436520497855L)]
    public class UserCountComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserCountComponent() { }
        public UserCountComponent(int userCount)
        {
            UserCount = userCount;
        }

        public int UserCount { get; set; }
    }
}