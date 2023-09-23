using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BitNet
{
    public class CLogicMessageEntry : IMessageDispatcher
    {
        CNetworkService service;
        ILogicQueue message_queue;
        AutoResetEvent logic_event;


        public CLogicMessageEntry(CNetworkService service)
        {
            this.service = service;
            this.message_queue = new CDoubleBufferingQueue();
            this.logic_event = new AutoResetEvent(false);
        }


        public void start()
        {
            Thread logic = new Thread(this.do_logic);
            logic.Start();
        }


        void IMessageDispatcher.on_message(CUserToken user, ArraySegment<byte> buffer)
        {
            CPacket msg = new CPacket(buffer, user);
            this.message_queue.enqueue(msg);

            this.logic_event.Set();
        }


        void do_logic()
        {
            while (true)
            {
                this.logic_event.WaitOne();

                dispatch_all(this.message_queue.get_all());
            }
        }


        void dispatch_all(Queue<CPacket> queue)
        {
            while (queue.Count > 0)
            {
                CPacket msg = queue.Dequeue();
                if (!this.service.usermanager.is_exist(msg.owner))
                {
                    continue;
                }

                msg.owner.on_message(msg);
            }
        }
    }
}
