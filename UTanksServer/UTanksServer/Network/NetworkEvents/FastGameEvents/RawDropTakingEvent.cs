using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.FastGameEvents
{
    public struct RawDropTakingEvent : INetSerializable
    {
        public int packetId;
        public long PlayerEntityId;
        public long dropEntityId;
        public Vector3S contactPosition;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerEntityId);
            writer.Push(dropEntityId);
            writer.Push(contactPosition);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerEntityId = reader.ReadInt64();
            dropEntityId = reader.ReadInt64();
            contactPosition = reader.ReadVector3();
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
