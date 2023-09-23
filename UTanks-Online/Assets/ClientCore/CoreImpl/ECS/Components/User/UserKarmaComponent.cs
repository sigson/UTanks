using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(190309489504464450)]
    public class UserKarmaComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserKarmaComponent() { }
        public UserKarmaComponent(int userKarma)
        {
            this.UserKarma = userKarma;
        }

        public int UserKarma { get; set; }
    }
}
