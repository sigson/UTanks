using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.User
{
    [TypeUidAttribute(190644533438150530)]
    public class UserLogged : ECSEvent
    {
        static public new long Id { get; set; }
        public ECSEntity userEntity;
        public UTanksServer.Network.Simple.Net.Server.User networkSocket;
        public bool userRelogin = false;
        [NonSerialized]
        [JsonIgnore]
        public Action<ECSEntity> actionAfterLoggin = (entity) => { };
        public override void Execute()
        {
            //throw new System.NotImplementedException();
        }
    }
}
