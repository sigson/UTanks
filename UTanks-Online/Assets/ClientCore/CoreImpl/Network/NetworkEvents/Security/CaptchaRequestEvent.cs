using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.Security
{
    public struct CaptchaRequiredEvent : INetSerializable
    {

        public void Deserialize(NetReader reader)
        {
        }

        public void Serialize(NetWriter writer)
        {
        }
    }
}
