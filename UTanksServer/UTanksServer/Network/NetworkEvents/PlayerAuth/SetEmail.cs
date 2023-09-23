using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerAuth
{
    public struct SetEmail : INetSerializable
    {
        public long uid;
        public string email;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            email = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(email);
        }
    }
}
