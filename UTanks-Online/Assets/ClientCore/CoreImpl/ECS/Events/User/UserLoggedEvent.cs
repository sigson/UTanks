using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.User
{
    [TypeUidAttribute(190644533438150530)]
    public class UserLogged : ECSEvent
    {
        static public new long Id { get; set; }
        public ECSEntity userEntity;
        public UTanksClient.Network.Simple.Net.Server.User networkSocket;
        public override void Execute()
        {
            //throw new System.NotImplementedException();
        }
    }
}
