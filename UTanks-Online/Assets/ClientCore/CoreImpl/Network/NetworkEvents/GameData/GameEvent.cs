using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Network.Simple.Net;

namespace Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData
{
    //this events automatically resending into ECSEventManager, in json store all needed data for ecs event
    public struct GameDataEvent : INetSerializable
    {
        public int packetId;
        public long typeId;
        public string jsonData;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(typeId);
            writer.Push(jsonData);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            typeId = reader.ReadInt64();
            jsonData = reader.ReadString();
        }
    }
}
