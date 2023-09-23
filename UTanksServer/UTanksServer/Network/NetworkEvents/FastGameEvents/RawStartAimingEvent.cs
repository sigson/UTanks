using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.FastGameEvents
{
    public struct RawStartAimingEvent : INetSerializable
    {
        public int packetId;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
