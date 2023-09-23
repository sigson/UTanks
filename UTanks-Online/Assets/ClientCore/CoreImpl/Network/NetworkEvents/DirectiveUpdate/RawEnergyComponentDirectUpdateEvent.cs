using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents
{
    public struct RawEnergyComponentDirectUpdateEvent : INetSerializable
    {
        public int packetId;
        public long PlayerUpdateEntityId;
        public long ComponentInstanceId;
        public float Energy;
        public float MaxEnergy;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerUpdateEntityId);
            writer.Push(ComponentInstanceId);
            writer.Push(Energy);
            writer.Push(MaxEnergy);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerUpdateEntityId = reader.ReadInt64();
            ComponentInstanceId = reader.ReadInt64();
            Energy = reader.ReadFloat();
            MaxEnergy = reader.ReadFloat();
        }
    }
}
