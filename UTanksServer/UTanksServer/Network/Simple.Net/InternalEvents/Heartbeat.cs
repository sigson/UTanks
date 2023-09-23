using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.Simple.Net.InternalEvents {
    public struct HeartBeat : INetSerializable {
        public long id;

        public void Serialize(NetWriter writer)
            => writer.Push(id);

        public void Deserialize(NetReader reader)
            => id = reader.ReadInt64();
    }
}