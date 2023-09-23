using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
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

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                UIService.instance.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().ChangeCrystalCount(UserCrystals);
            }, null);
        }
    }
}
