using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(196562600580045340)]
    public class UserEmailComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserEmailComponent() { }
        public UserEmailComponent(string email, bool emailVerified)
        {
            this.Email = email;
            this.EmailVerified = emailVerified;
        }

        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}
