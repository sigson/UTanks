using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.FastGameEvents
{
    public struct RawCreatureActuationEvent : INetSerializable
    {
        public int packetId;
        public long BattleDBOwnerId;
        public long CreatureInstanceId;
        public List<long> TargetsId;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(BattleDBOwnerId);
            writer.Push(CreatureInstanceId);
            writer.Push(TargetsId, writer.Push);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            BattleDBOwnerId = reader.ReadInt64();
            CreatureInstanceId = reader.ReadInt64();
            TargetsId = reader.ReadList(reader.ReadInt64);
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
