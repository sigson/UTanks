using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.PlayerSettings
{
    public struct SetAvatar : INetSerializable
    {
        public long uid;
        public string avatar;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            avatar = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(avatar);
        }
    }
}
