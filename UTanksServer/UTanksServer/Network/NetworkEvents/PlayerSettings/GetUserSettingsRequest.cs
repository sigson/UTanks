using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerSettings
{
    public struct GetUserSettingsRequest : INetSerializable
    {
        public int packetId;
        public long uid;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(uid);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            uid = reader.ReadInt64();
        }
    }
}
