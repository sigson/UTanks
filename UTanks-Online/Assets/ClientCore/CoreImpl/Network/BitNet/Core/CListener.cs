using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BitNet
{
	class CListener
	{
		SocketAsyncEventArgs accept_args;

		Socket listen_socket;

		AutoResetEvent flow_control_event;

		public delegate void NewclientHandler(Socket client_socket, object token);
		public NewclientHandler callback_on_newclient;

        public CListener()
        {
			this.callback_on_newclient = null;
        }

		public void start(string host, int port, int backlog)
		{
			this.listen_socket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			IPAddress address;
			if (host == "0.0.0.0")
			{
				address = IPAddress.Any;
			}
			else
			{
				address = IPAddress.Parse(host);
			}
			IPEndPoint endpoint = new IPEndPoint(address, port);

			try
			{
				listen_socket.Bind(endpoint);
				listen_socket.Listen(backlog);

				this.accept_args = new SocketAsyncEventArgs();
				this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

                Thread listen_thread = new Thread(do_listen);
                listen_thread.Start();
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.Message);
			}
		}

		void do_listen()
		{
            this.flow_control_event = new AutoResetEvent(false);

			while (true)
			{
				this.accept_args.AcceptSocket = null;

				bool pending = true;
				try
				{
					pending = listen_socket.AcceptAsync(this.accept_args);
				}
				catch (Exception e)
				{
					//Console.WriteLine(e.Message);
					continue;
				}

				if (!pending)
				{
					on_accept_completed(null, this.accept_args);
				}

				this.flow_control_event.WaitOne();

			}
		}

		void on_accept_completed(object sender, SocketAsyncEventArgs e)
		{
            if (e.SocketError == SocketError.Success)
            {
                Socket client_socket = e.AcceptSocket;
                client_socket.NoDelay = true;

                if (this.callback_on_newclient != null)
                {
                    this.callback_on_newclient(client_socket, e.UserToken);
                }

                this.flow_control_event.Set();

                return;
            }
            else
            {
				Console.WriteLine("Failed to accept client. " + e.SocketError);
            }

            this.flow_control_event.Set();
		}
	}
}
