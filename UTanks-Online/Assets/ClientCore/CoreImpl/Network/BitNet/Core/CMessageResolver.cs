using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitNet
{
   	class Defines
	{
		public static readonly short HEADERSIZE = 4;
	}

    public delegate void CompletedMessageCallback(ArraySegment<byte> buffer);

    class CMessageResolver
	{
		int message_size;

		byte[] message_buffer = new byte[1024];

		int current_position;

		int position_to_read;

		int remain_bytes;

		public CMessageResolver()
		{
			this.message_size = 0;
			this.current_position = 0;
			this.position_to_read = 0;
			this.remain_bytes = 0;
		}

		bool read_until(byte[] buffer, ref int src_position)
		{
            int copy_size = this.position_to_read - this.current_position;

			if (this.remain_bytes < copy_size)
			{
				copy_size = this.remain_bytes;
			}

            Array.Copy(buffer, src_position, this.message_buffer, this.current_position, copy_size);

			src_position += copy_size;

			this.current_position += copy_size;

			this.remain_bytes -= copy_size;

			if (this.current_position < this.position_to_read)
			{
				return false;
			}

			return true;
		}

		public void on_receive(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback)
		{
			this.remain_bytes = transffered;

			int src_position = offset;

			while (this.remain_bytes > 0)
			{
				bool completed = false;

				if (this.current_position < Defines.HEADERSIZE)
				{
					this.position_to_read = Defines.HEADERSIZE;

                    completed = read_until(buffer, ref src_position);
					if (!completed)
					{
						return;
					}

                    this.message_size = get_total_message_size();

                    // It was wrong message if size less than zero.
                    if (this.message_size <= 0)
                    {
                        clear_buffer();
                        return;
                    }

                    this.position_to_read = this.message_size;

                    if (this.remain_bytes <= 0)
                    {
                        return;
                    }
                }

                completed = read_until(buffer, ref src_position);

				if (completed)
				{
                    byte[] clone = new byte[this.position_to_read];
                    Array.Copy(this.message_buffer, clone, this.position_to_read);
                    clear_buffer();
                    callback(new ArraySegment<byte>(clone, 0, this.position_to_read));
                }
			}
		}

		int get_total_message_size()
		{
            if (Defines.HEADERSIZE == 2)
            {
                return BitConverter.ToInt16(this.message_buffer, 0);
            }
            else if (Defines.HEADERSIZE == 4)
            {
                return BitConverter.ToInt32(this.message_buffer, 0);
            }

            return 0;
		}

		public void clear_buffer()
		{
			Array.Clear(this.message_buffer, 0, this.message_buffer.Length);

			this.current_position = 0;
			this.message_size = 0;

		}
	}
}
