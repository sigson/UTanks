using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Core;

namespace BitNet
{
    public class CUserToken
    {
        enum State
        {
            Idle,

            Connected,

            ReserveClosing,

            Closed,
        }

        const short SYS_CLOSE_REQ = 0;
        const short SYS_CLOSE_ACK = -1;
        public const short SYS_START_HEARTBEAT = -2;
        public const short SYS_UPDATE_HEARTBEAT = -3;

        int is_closed;

        State current_state;
        public Socket socket { get; set; }

        public SocketAsyncEventArgs receive_event_args { get; private set; }
        public SocketAsyncEventArgs send_event_args { get; private set; }

        CMessageResolver message_resolver;

        IPeer peer;

        List<ArraySegment<byte>> sending_list;
        private object cs_sending_queue;

        IMessageDispatcher dispatcher;

        public delegate void ClosedDelegate(CUserToken token);
        public ClosedDelegate on_session_closed;

        // heartbeat.
        public long latest_heartbeat_time { get; private set; }
        CHeartbeatSender heartbeat_sender;
        bool auto_heartbeat;


        public CUserToken(IMessageDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.cs_sending_queue = new object();

            this.message_resolver = new CMessageResolver();
            this.peer = null;
            this.sending_list = new List<ArraySegment<byte>>();
            this.latest_heartbeat_time = DateTime.Now.Ticks;

            this.current_state = State.Idle;
        }

        public void on_connected()
        {
            this.current_state = State.Connected;
            this.is_closed = 0;
            this.auto_heartbeat = true;
        }

        public void set_peer(IPeer peer)
        {
            this.peer = peer;
        }

        public void set_event_args(SocketAsyncEventArgs receive_event_args, SocketAsyncEventArgs send_event_args)
        {
            this.receive_event_args = receive_event_args;
            this.send_event_args = send_event_args;
        }

        public void on_receive(byte[] buffer, int offset, int transfered)
        {
            this.message_resolver.on_receive(buffer, offset, transfered, on_message_completed);
        }

        void on_message_completed(ArraySegment<byte> buffer)
        {
            if (this.peer == null)
            {
                return;
            }

            if (this.dispatcher != null)
            {
                this.dispatcher.on_message(this, buffer);
            }
            else
            {
                CPacket msg = new CPacket(buffer, this);
                on_message(msg);
            }
        }


        public void on_message(CPacket msg)
        {
            switch (msg.protocol_id)
            {
                case SYS_CLOSE_REQ:
                    disconnect();
                    return;

                case SYS_START_HEARTBEAT:
                    {
                        msg.pop_protocol_id();
                        byte interval = msg.pop_byte();
                        this.heartbeat_sender = new CHeartbeatSender(this, interval);

                        if (this.auto_heartbeat)
                        {
                            start_heartbeat();
                        }
                    }
                    return;

                case SYS_UPDATE_HEARTBEAT:
                    //Console.WriteLine("heartbeat : " + DateTime.Now);
                    this.latest_heartbeat_time = DateTime.Now.Ticks;
                    return;
            }


            if (this.peer != null)
            {
                try
                {
                    switch (msg.protocol_id)
                    {
                        case SYS_CLOSE_ACK:
                            this.peer.on_removed();
                            break;

                        default:
                            this.peer.on_message(msg);
                            break;
                    }
                }
                catch (Exception)
                {
                    close();
                    Logger.LogError("error data parsing, close");
                }
            }

            if (msg.protocol_id == SYS_CLOSE_ACK)
            {
                if (this.on_session_closed != null)
                {
                    this.on_session_closed(this);
                }
            }
        }

        public void close()
        {
            if (Interlocked.CompareExchange(ref this.is_closed, 1, 0) == 1)
            {
                return;
            }

            if (this.current_state == State.Closed)
            {
                return;
            }

            this.current_state = State.Closed;
            this.socket.Close();
            this.socket = null;

            this.send_event_args.UserToken = null;
            this.receive_event_args.UserToken = null;

            this.sending_list.Clear();
            this.message_resolver.clear_buffer();

            if (this.peer != null)
            {
                CPacket msg = CPacket.create((short)-1);
                if (this.dispatcher != null)
                {
                    this.dispatcher.on_message(this, new ArraySegment<byte>(msg.buffer, 0, msg.position));
                }
                else
                {
                    on_message(msg);
                }
            }
        }


        public void send(ArraySegment<byte> data)
        {
            lock (this.cs_sending_queue)
            {
                this.sending_list.Add(data);

                if (this.sending_list.Count > 1)
                {
                    return;
                }
            }

            start_send();
        }


        public void send(CPacket msg)
        {
            msg.record_size();
            send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
        }


        void start_send()
        {
            try
            {
                this.send_event_args.BufferList = this.sending_list;

                bool pending = this.socket.SendAsync(this.send_event_args);
                if (!pending)
                {
                    process_send(this.send_event_args);
                }
            }
            catch (Exception e)
            {
                if (this.socket == null)
                {
                    close();
                    return;
                }

                Console.WriteLine("send error!! close socket. " + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        static int sent_count = 0;
        static object cs_count = new object();

        public void process_send(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                //Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
                return;
            }

            lock (this.cs_sending_queue)
            {
                var size = this.sending_list.Sum(obj => obj.Count);

                if (e.BytesTransferred != size)
                {
                    if (e.BytesTransferred < this.sending_list[0].Count)
                    {
                        string error = string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, size);
                        Console.WriteLine(error);

                        close();
                        return;
                    }

                    int sent_index = 0;
                    int sum = 0;
                    for (int i = 0; i < this.sending_list.Count; ++i)
                    {
                        sum += this.sending_list[i].Count;
                        if (sum <= e.BytesTransferred)
                        {
                            sent_index = i;
                            continue;
                        }

                        break;
                    }
                    this.sending_list.RemoveRange(0, sent_index + 1);

                    start_send();
                    return;
                }

                this.sending_list.Clear();
 
                if (this.current_state == State.ReserveClosing)
                {
                    this.socket.Shutdown(SocketShutdown.Send);
                }
            }
        }


        public void disconnect()
        {
            // close the socket associated with the client
            try
            {
                if (this.sending_list.Count <= 0)
                {
                    this.socket.Shutdown(SocketShutdown.Send);
                    return;
                }

                this.current_state = State.ReserveClosing;
            }
            // throws if client process has already closed
            catch (Exception)
            {
                close();
            }
        }


        public void ban()
        {
            try
            {
                byebye();
            }
            catch (Exception)
            {
                close();
            }
        }


        void byebye()
        {
            CPacket bye = CPacket.create(SYS_CLOSE_REQ);
            send(bye);
        }


        public bool is_connected()
        {
            return this.current_state == State.Connected;
        }


        public void start_heartbeat()
        {
            if (this.heartbeat_sender != null)
            {
                this.heartbeat_sender.play();
            }
        }


        public void stop_heartbeat()
        {
            if (this.heartbeat_sender != null)
            {
                this.heartbeat_sender.stop();
            }
        }


        public void disable_auto_heartbeat()
        {
            stop_heartbeat();
            this.auto_heartbeat = false;
        }


        public void update_heartbeat_manually(float time)
        {
            if (this.heartbeat_sender != null)
            {
                this.heartbeat_sender.update(time);
            }
        }
    }
}
