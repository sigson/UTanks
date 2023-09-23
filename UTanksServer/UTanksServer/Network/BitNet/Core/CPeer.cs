using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitNet
{
	class CPeer
	{
		public void on_message(Const<byte[]> buffer)
		{
			CPacket msg = new CPacket(buffer.Value, null);
			Int16 protocol_id = msg.pop_int16();
			switch (protocol_id)
			{
				case 1:
					{
						Int32 number = msg.pop_int32();
						string text = msg.pop_string();

						Console.WriteLine(string.Format("[{0}] [received] {1} : {2}, {3}",
							System.Threading.Thread.CurrentThread.ManagedThreadId,
							protocol_id, number, text));
					}
					break;
			}
		}
	}
}
