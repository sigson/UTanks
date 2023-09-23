﻿using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.Security
{
    public struct CaptchaRequiredEvent : INetSerializable
    {
        public string captchaImg;

        public void Deserialize(NetReader reader)
        {
            captchaImg = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(captchaImg);
        }
    }
}
