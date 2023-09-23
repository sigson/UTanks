using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(187747923072293220)]
    public class UsernameComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UsernameComponent() { }
        public UsernameComponent(string username)
        {
            this.Username = username;
        }

        public string Username { get; set; }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                UIService.instance.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().ChangeUsername(Username);
            }, null);
        }
    }
}
