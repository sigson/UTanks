using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Chat
{
    [TypeUidAttribute(210539226954322430)]
    public class ChatMessageCallbackEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long messageInstanceId = 0;
        public string callbackBody = "";
        public override void Execute()
        {
            //throw new System.NotImplementedException();
        }
    }
}
