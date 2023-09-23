using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Communications
{
    public struct RestoreConnection : INetSerializable
    {
        public long uid;

        public void Serialize(NetWriter writer)
            => writer.Push(uid);

        public void Deserialize(NetReader reader)
            => uid = reader.ReadInt64();
    }
}
