using System;

namespace UTanksServer.Network.Simple.Net {
    public interface INetSerializable {
        void Serialize(NetWriter buffer);
        void Deserialize(NetReader buffer);
    }
}