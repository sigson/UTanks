using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerSettings
{
    public struct SetSubscribed : INetSerializable
    {
        public long uid;
        public bool state;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            state = reader.ReadBool();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(state);
        }
    }
}
