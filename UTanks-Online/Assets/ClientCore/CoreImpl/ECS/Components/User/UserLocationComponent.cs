using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(202959332570274880)]
    public class UserLocationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserLocationComponent() { }
        public UserLocationComponent(string userLocation)
        {
            this.UserLocation = UserLocation;
        }

        public string UserLocation { get; set; }
    }
}
