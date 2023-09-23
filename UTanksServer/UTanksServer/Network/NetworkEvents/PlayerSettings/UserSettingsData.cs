using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerSettings
{
    public struct UserSettingsData : INetSerializable
    {
        public int packetId;
        public string countryCode;
        public string avatar;
        public long premiumExpiration;
        public bool subscribed;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(countryCode);
            writer.Push(avatar);
            writer.Push(premiumExpiration);
            writer.Push(subscribed);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            countryCode = reader.ReadString();
            avatar = reader.ReadString();
            premiumExpiration = reader.ReadInt64();
            subscribed = reader.ReadBool();
        }
    }
}
