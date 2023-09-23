using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.Communications
{
    public struct StartCommunication : INetSerializable
    {
        public void Serialize(NetWriter writer) { }
        public void Deserialize(NetReader reader) { }
    }
}
