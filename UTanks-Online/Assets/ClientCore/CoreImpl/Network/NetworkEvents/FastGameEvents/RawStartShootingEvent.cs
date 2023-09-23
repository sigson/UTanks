using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.FastGameEvents
{
    public struct RawStartShootingEvent : INetSerializable
    {
        public int packetId;
        public long PlayerEntityId;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerEntityId);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerEntityId = reader.ReadInt64();
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
