using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.Communications
{
    public struct CheckConfigVersion : INetSerializable
    {
        public long uid;
        public long hash;

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(hash);
        }

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            hash = reader.ReadInt64();
        }
    }
}
