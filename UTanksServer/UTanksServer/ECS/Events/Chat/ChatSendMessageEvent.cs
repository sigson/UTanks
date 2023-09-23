using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Chat
{
    [TypeUidAttribute(187126171713431000)]
    public class ChatSendMessageEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long battleEntity = 0;
        public long channelEntity = 0;
        public bool teamMessage = false;
        public string messageBody = "";
        public string SenderNickname = "";
        public int SenderRank = 0;
        public override void Execute()
        {
            //throw new System.NotImplementedException();
        }
    }
}
