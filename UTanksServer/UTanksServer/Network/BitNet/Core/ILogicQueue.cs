using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitNet
{
    public interface ILogicQueue
    {
        void enqueue(CPacket msg);
        Queue<CPacket> get_all();
    }
}
