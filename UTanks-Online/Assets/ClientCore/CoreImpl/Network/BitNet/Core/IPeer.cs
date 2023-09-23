using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace BitNet
{

	public interface IPeer
	{

        void on_message(CPacket msg);

        void on_removed();


		void send(CPacket msg);


		void disconnect();
    }
}
