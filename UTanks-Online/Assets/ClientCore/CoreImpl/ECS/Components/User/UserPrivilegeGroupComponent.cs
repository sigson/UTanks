using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1503469594706)]
    public class UserPrivilegeGroupComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }


        public UserPrivilegeGroupComponent()
        {
        }

        public UserPrivilegeGroupComponent(string privilegeGroup)
        {
            PrivilegeGroup = privilegeGroup;
        }

        public string PrivilegeGroup { get; set; }
    }
}