using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.PlayerAuth
{
    public struct AvailableResult : INetSerializable
    {
        public int packetId;
        public bool result;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(result);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            result = reader.ReadBool();
        }
    }
}
