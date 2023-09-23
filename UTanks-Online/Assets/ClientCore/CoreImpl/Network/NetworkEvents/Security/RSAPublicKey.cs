using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Security {
    public struct RSAPublicKey : INetSerializable {
        public string Key;

        public void Deserialize(NetReader reader)
            => Key = reader.ReadString();

        public void Serialize(NetWriter writer)
            => writer.Push(Key);
    }
}
