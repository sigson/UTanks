using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.PlayerAuth
{
    public struct SetHashedPassword : INetSerializable
    {
        public long uid;
        public string hashedPassword;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            hashedPassword = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(hashedPassword);
        }
    }
}
