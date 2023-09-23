using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UTanksServer.Network.Simple.Net.InternalEvents;
using UTanksServer.Network.Simple.Net;
using System.Collections.Concurrent;
using UTanksServer.Database;
using BitNet;
using System.Net;

namespace UTanksServer.Network.Simple.Net.Client {
    public class Client : IPeer
    {
        public TcpClient socket { get; private set; }
        NetworkStream networkStream;

        public CUserToken token;
        public CNetworkService service;
        public CConnector connector;

        public bool Connected { get => token != null ? token.is_connected() : false; }
        byte[] buffer;

        Action onConnect;
        Action onDisconnect;
        Dictionary<long, object> Events = new Dictionary<long, object>();
        ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>> SlicedPackets = new ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>>();

        string host;
        int port;
        public RSADecryptComponent RSADecryptionComponent = new RSADecryptComponent();
        public RSAEncryptCompoenent RSAEncryptionComponent; // Defined in Authenticator.Process => user.on<RSAPublicKey>

        public Client(string host, int port, Action onConnect, Action onDisconnect, int bufferSize = 2048) {
            socket = new TcpClient() {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            service = new CNetworkService(true);

            connector = new CConnector(service);
            connector.connected_callback += OnConnect;
            

            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            buffer = new byte[bufferSize];

            this.host = host;
            this.port = port;
            
            on<HeartBeat>((HeartBeat packet) => emit(packet));
        }

        public void Connect()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(this.host), this.port);
            connector.connect(endpoint);
        }

        void OnConnect(CUserToken server_token) {
            this.token = server_token;
            this.token.set_peer(this);
            server_token.on_connected();
            this.onConnect();
        }

        void IPeer.on_message(CPacket msg)
        {
            msg.pop_protocol_id();
            OnReceive(msg.pop_bytepack());
        }

        void IPeer.on_removed()
        {
            Disconnect();
        }

        void IPeer.send(CPacket msg)
        {
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
        }

        void IPeer.disconnect()
        {
            this.token.ban();
            Disconnect();
        }


        void OnReceive(byte[] newBuffer) {
            if (newBuffer.Length == 0) {
                Disconnect();
                return;
            }

            //Array.Copy(buffer, newBuffer, newBuffer.Length);
            
            NetReader reader = new NetReader(newBuffer);

            if (reader.length > Networking.server.bufferSize)
            {
                ConcurrentDictionary<long, byte[]> bufPackets;
                var getResult = SlicedPackets.TryGetValue(reader.id, out bufPackets);
                if (getResult && reader.length - bufPackets.Count * Networking.server.bufferSize <= Networking.server.bufferSize)
                {
                    bufPackets.TryAdd(reader.packetPosition, reader.buffer.ToArray());
                    reader.buffer = new List<byte>();
                    for (int i = 0; i < bufPackets.Count; i++)
                    {
                        if (i == 0)
                            reader.buffer.AddRange(bufPackets[i]);
                        else
                        {
                            reader.buffer.AddRange(bufPackets[i].SubArray(8 * 4, bufPackets[i].Length - 8 * 4));
                        }
                    }
                    SlicedPackets.TryRemove(reader.id, out _);
                }
                else
                {
                    if (getResult)
                    {
                        bufPackets.TryAdd(reader.packetPosition, reader.buffer.ToArray());
                    }
                    else
                    {
                        bufPackets = new ConcurrentDictionary<long, byte[]>();
                        SlicedPackets.TryAdd(reader.id, bufPackets);
                        bufPackets.TryAdd(reader.packetPosition, reader.buffer.ToArray());
                    }
                }
                if (SlicedPackets.TryGetValue(reader.id, out _))
                {
                    return;
                }
            }

            #region old

            //if (reader.length > Networking.server.bufferSize)
            //{
            //    ConcurrentDictionary<long, byte[]> bufPackets;
            //    if(SlicedPackets.TryGetValue(reader.id, out bufPackets))
            //    {
            //        bufPackets.TryAdd(reader.packetPosition, reader.buffer.ToArray());
            //    }
            //    else
            //    {
            //        bufPackets = new ConcurrentDictionary<long, byte[]>();
            //        SlicedPackets.TryAdd(reader.id, bufPackets);
            //        bufPackets.TryAdd(reader.packetPosition, reader.buffer.ToArray());
            //    }
            //    if(bufPackets.Count * 2048 - reader.length <= 2048)
            //    {
            //        reader.buffer = new List<byte>();
            //        for (int i = 0; i < bufPackets.Count; i++)
            //        {
            //            reader.buffer.AddRange(bufPackets[i]);
            //        }
            //    }
            //    else
            //    {
            //        try { networkStream.BeginRead(buffer, 0, buffer.Length, OnReceive, null); }
            //        catch { return; }
            //        return;
            //    }
                
            //}

            #endregion

            Type eventType = HashCache.GetType(reader.hashCode);
            if (eventType == null) {
                Console.WriteLine($"ERR: Unknown event code [hash: {reader.hashCode}]");
                return;
            }
            if (!Events.ContainsKey(reader.hashCode)) {
                Console.WriteLine($"ERR: Got packet with no callback [name: {eventType.Name}, hash: {reader.hashCode}]");
                return;
            }

            dynamic packetInstance = Activator.CreateInstance(eventType);
            packetInstance.Deserialize(reader);
            
            object eventHandler = Events[reader.hashCode];
            Type callbackType = eventHandler.GetType();
            if (callbackType == typeof(Action))
                ((Action)eventHandler)();
            else if (callbackType == typeof(Action<Client>))
                ((Action<Client>)eventHandler)(this);
            else if (callbackType == typeof(Action<object>))
                ((Action<object>)eventHandler)(packetInstance);
            else if (callbackType == typeof(Action<Client, object>))
                ((Action<Client, object>)eventHandler)(this, packetInstance);
            else Console.WriteLine($"Got unknown event callback for packet! [name: {eventType.Name}, hash: {reader.hashCode}, callback: {eventHandler}]");

        }

        public void emit<T>(T packet) where T : struct, INetSerializable {
            NetWriter writer = new NetWriter(HashCache.Get<T>());
            packet.Serialize(writer);
            var byteBuffer = new List<byte>(writer.ToByteArray());
            int position = 0;
            CPacket cPacket = null;
            while ((((float)byteBuffer.Count) / ((float)Networking.server.bufferSize)) - position - 1 > 0)
            {
                cPacket = CPacket.create((short)PROTOCOL.Server);
                cPacket.push(byteBuffer.GetRange(position * Networking.server.bufferSize, Networking.server.bufferSize).ToArray());
                this.token.send(cPacket);
                position++;
            }
            cPacket = CPacket.create((short)PROTOCOL.Server);
            cPacket.push(byteBuffer.GetRange(position * Networking.server.bufferSize, byteBuffer.Count - position * Networking.server.bufferSize).ToArray());
            this.token.send(cPacket);
        }

        public bool eventExists<T>() where T : struct, INetSerializable
            => Events.ContainsKey(HashCache.Get<T>());

        public void on<T>(Action callback) where T : struct, INetSerializable {
            if (callback == null) return;

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }

        public void on<T>(Action<Client> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }
        
        public void on<T>(Action<T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => callback((T)o));
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => callback((T)o)));
        }
        
        public void on<T>(Action<Client, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => callback(this, (T)o));
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => callback(this, (T)o)));
        }

        public void onOnce<T>(Action<Client> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action(() => { removeEvent<T>(); callback(this);});
            else Events.Add(HashCache.Get<T>(), new Action(() => { removeEvent<T>(); callback(this);}));
        }
        
        public void onOnce<T>(Action<T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => { removeEvent<T>(); callback((T)o);});
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => { removeEvent<T>(); callback((T)o);}));
        }
        
        public void onOnce<T>(Action<Client, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); });
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); }));
        }

        public void removeEvent<T>() where T : struct, INetSerializable {
            if (eventExists<T>())
                Events.Remove(HashCache.Get<T>());
        }

        public void Disconnect() {
            if (Connected) {
                token.close();
                onDisconnect();
            }
        }
    }
}