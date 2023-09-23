using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.Simple.Net.InternalEvents {
    public struct HeartBeat : INetSerializable {
        public long id;

        public void Serialize(NetWriter writer)
            => writer.Push(id);

        public void Deserialize(NetReader reader)
            => id = reader.ReadInt64();
    }
}