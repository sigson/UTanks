using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Communications {
    public struct UserLoggedInEvent : INetSerializable
    {
        public long uid;
        public string Username;
        public long entityId;
        public long serverTime;

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(Username);
            writer.Push(entityId);
            writer.Push(serverTime);
        }

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            Username = reader.ReadString();
            entityId = reader.ReadInt64();
            serverTime = reader.ReadInt64();
        }
    }
}
