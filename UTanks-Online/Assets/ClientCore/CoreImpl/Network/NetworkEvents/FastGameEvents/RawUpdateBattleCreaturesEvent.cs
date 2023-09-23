using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.FastGameEvents
{
    public struct RawUpdateBattleCreaturesEvent : INetSerializable
    {
        public int packetId;
        public long BattleId;
        public Dictionary<long, List<ICreatureComponent>> appendCreatures;
        public Dictionary<long, List<long>> removeCreatures;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(BattleId);
            writer.Push(appendCreatures, writer.Push, (creatures) =>
            {
                writer.Push(creatures, writer.Push);
            });
            writer.Push(removeCreatures, writer.Push, (creatures) =>
            {
                writer.Push(creatures, writer.Push);
            });
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            BattleId = reader.ReadInt64();
            appendCreatures = reader.ReadDictionary<long, List<ICreatureComponent>>(reader.ReadInt64, () => {
                return reader.ReadList<ICreatureComponent>(reader.ReadICreatureComponent);
            });
            removeCreatures = reader.ReadDictionary<long, List<long>>(reader.ReadInt64, () => {
                return reader.ReadList(reader.ReadInt64);
            });
        }
    }
}
