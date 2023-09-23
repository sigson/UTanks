using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Security {
    public struct LoginFailedEvent : INetSerializable {
        public string reason;

        public void Deserialize(NetReader reader)
            => reason = reader.ReadString();

        public void Serialize(NetWriter writer)
            => writer.Push(reason);
    }
}
