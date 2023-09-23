using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.PlayerAuth
{
    public struct RegisterUserRequest : INetSerializable
    {
        public int packetId;
        public string Username;
        public string Password;
        public string Email;
        public string HardwareId;
        public string HardwareToken;
        public string CountryCode;
        public bool subscribed;
        public string captchaResultHash;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(Username);
            writer.Push(Password);
            writer.Push(Email);
            writer.Push(HardwareId);
            writer.Push(HardwareToken);
            writer.Push(CountryCode);
            writer.Push(subscribed);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            Username = reader.ReadString();
            Password = reader.ReadString();
            Email = reader.ReadString();
            HardwareId = reader.ReadString();
            HardwareToken = reader.ReadString();
            CountryCode = reader.ReadString();
            subscribed = reader.ReadBool();
        }
    }
}
