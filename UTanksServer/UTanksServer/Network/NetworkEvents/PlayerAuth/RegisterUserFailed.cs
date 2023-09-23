using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.PlayerAuth
{
    public struct RegisterUserFailed : INetSerializable
    {
        public string reason;

        public void Deserialize(NetReader reader)
            => reason = reader.ReadString();

        public void Serialize(NetWriter writer)
            => writer.Push(reason);
    }
}
