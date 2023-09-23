using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents
{
    public struct RawHealthComponentDirectUpdateEvent : INetSerializable
    {
        public int packetId;
        public long PlayerUpdateEntityId;
        public long ComponentInstanceId;
        public float CurrentHealth;
        public float MaxHealth;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerUpdateEntityId);
            writer.Push(ComponentInstanceId);
            writer.Push(CurrentHealth);
            writer.Push(MaxHealth);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerUpdateEntityId = reader.ReadInt64();
            ComponentInstanceId = reader.ReadInt64();
            CurrentHealth = reader.ReadFloat();
            MaxHealth = reader.ReadFloat();
        }
    }
}
