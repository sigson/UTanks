﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.FastGameEvents
{
    public struct RawSupplyUsingEvent : INetSerializable
    {
        public int packetId;
        public long PlayerEntityId;
        public string supplyPath;
        public List<long> targetEntities;
        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerEntityId);
            writer.Push(supplyPath);
            writer.Push(targetEntities, writer.Push);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerEntityId = reader.ReadInt64();
            supplyPath = reader.ReadString();
            targetEntities = reader.ReadList(reader.ReadInt64);
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
