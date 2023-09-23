using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.PlayerAuth
{
    public struct SetUserRememberMeCredentials : INetSerializable
    {
        public long uid;
        public string hardwareId;
        public string hardwareToken;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            hardwareId = reader.ReadString();
            hardwareToken = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(hardwareId);
            writer.Push(hardwareToken);
        }
    }
}
