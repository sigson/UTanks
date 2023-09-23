using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Security {
    public struct LoginSuccessEvent : INetSerializable {
        public void Serialize(NetWriter writer) {}
        public void Deserialize(NetReader reader) {}
    }
}
