using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.FastGameEvents
{
    public struct RawHitEvent : INetSerializable
    {
        public int packetId;
        public long PlayerEntityId;
        public Dictionary<long, Vector3S> hitList;
        public Dictionary<long, float> hitDistanceList;
        public Dictionary<long, float> hitLocalDistanceList;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerEntityId);
            writer.Push(hitList, writer.Push, writer.Push);
            writer.Push(hitDistanceList, writer.Push, writer.Push);
            writer.Push(hitLocalDistanceList, writer.Push, writer.Push);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerEntityId = reader.ReadInt64();
            hitList = reader.ReadDictionary(reader.ReadInt64, reader.ReadVector3);
            hitDistanceList = reader.ReadDictionary(reader.ReadInt64, reader.ReadFloat);
            hitLocalDistanceList = reader.ReadDictionary(reader.ReadInt64, reader.ReadFloat);
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
