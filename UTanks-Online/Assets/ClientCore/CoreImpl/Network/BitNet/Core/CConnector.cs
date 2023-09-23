using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BitNet
{
	public class CConnector
	{
		public delegate void ConnectedHandler(CUserToken token);
		public ConnectedHandler connected_callback { get; set; }

		Socket client;

		CNetworkService network_service;

		public CConnector(CNetworkService network_service)
		{
			this.network_service = network_service;
			this.connected_callback = null;
		}

		public void connect(IPEndPoint remote_endpoint)
		{
			this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.client.NoDelay = true;

			SocketAsyncEventArgs event_arg = new SocketAsyncEventArgs();
			event_arg.Completed += on_connect_completed;
			event_arg.RemoteEndPoint = remote_endpoint;
			bool pending = this.client.ConnectAsync(event_arg);
			if (!pending)
			{
				on_connect_completed(null, event_arg);
			}
		}

		public void disconnect()
		{
			this.client.Close();
		}

		void on_connect_completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				//Console.WriteLine("Connect completd!");
				CUserToken token = new CUserToken(this.network_service.logic_entry);

                this.network_service.on_connect_completed(this.client, token);

				if (this.connected_callback != null)
				{
					this.connected_callback(token);
				}
			}
			else
			{
				// failed.
				Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
			}
		}
	}
}
