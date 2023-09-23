using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerAuth
{
    public struct SetEmailVerified : INetSerializable
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
