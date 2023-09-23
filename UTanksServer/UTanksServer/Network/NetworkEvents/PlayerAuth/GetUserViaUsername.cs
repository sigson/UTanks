using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerAuth
{
    public struct GetUserViaUsername : INetSerializable
    {
        public int packetId;
        public string Username;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(Username);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            Username = reader.ReadString();
        }
    }
}
