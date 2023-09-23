using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(205313947056851070)]
    public class ChatBanEndTimeComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public ChatBanEndTimeComponent()
        { }
        public ChatBanEndTimeComponent(long banTimeExpiration)
        {
            this.banTimeExpiration = banTimeExpiration;
        }

        public long? banTimeExpiration { get; set; } = null;
    }
}
