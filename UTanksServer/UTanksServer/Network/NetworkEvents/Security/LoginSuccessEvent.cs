using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.Security {
    public struct LoginSuccessEvent : INetSerializable {
        public void Serialize(NetWriter writer) {}
        public void Deserialize(NetReader reader) {}
    }
}
