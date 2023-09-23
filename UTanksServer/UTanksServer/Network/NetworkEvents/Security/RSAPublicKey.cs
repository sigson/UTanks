using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.Security {
    public struct RSAPublicKey : INetSerializable {
        public string Key;

        public void Deserialize(NetReader reader)
            => Key = reader.ReadString();

        public void Serialize(NetWriter writer)
            => writer.Push(Key);
    }
}
