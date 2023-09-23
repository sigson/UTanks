using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1473074767785)]
    public class UserCrystalsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserCrystalsComponent() { }
        public UserCrystalsComponent(int userCrystals)
        {
            this.UserCrystals = userCrystals;
        }

        public int UserCrystals { get; set; }
    }
}
