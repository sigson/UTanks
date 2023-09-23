using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.User
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
    }
}
