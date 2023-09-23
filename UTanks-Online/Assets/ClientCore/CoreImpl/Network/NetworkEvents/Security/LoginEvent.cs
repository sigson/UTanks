using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Security {
    public struct LoginEvent : INetSerializable {
        public string login; // eg: D465-P634-UI41-T015
        public string password; // eg: 143-438-129-432-102-525
        public string captchaResultHash;
        public string captchaImg;

        public void Deserialize(NetReader reader) {
            login = reader.ReadString();
            password = reader.ReadString();
            captchaResultHash = reader.ReadString();
            captchaImg = reader.ReadString();
        }

        public void Serialize(NetWriter writer) {
            writer.Push(login);
            writer.Push(password);
            writer.Push(captchaResultHash);
            writer.Push(captchaImg);
        }
    }
}
