using System;
using System.Threading;
//using System.Timers;
using System.Collections.Generic;
using System.Net.Sockets;
using SimpleJSON;
using UTanksServer.Network.Simple.Net.InternalEvents;
using Core;
using UTanksServer.Database;
using System.Collections.Concurrent;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using BitNet;

namespace UTanksServer.Network.Simple.Net.Server {
    public class User : IPeer
    {
        public CUserToken token;
        public int clientId { get; protected set; }
        static int ClientIdCounter = 0;
        public long PlayerEntityId;
        public object sendLocker = new object();
        public object receiveLocker = new object();
        public bool SocketClosed;
        public int errorCount;//after no error - clear
        public Timer heartBeat;
        long pingId = 0;
        long lastResponsesPingId = 0;

        public RSADecryptComponent RSADecryptionComponent = new RSADecryptComponent();
        public RSAEncryptCompoenent RSAEncryptionComponent; // Defined in Authenticator.Process => user.on<RSAPublicKey>

        ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>> SlicedPackets = new ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>>();

        public Socket socket { get; protected set; }
        public JSONNode data = new JSONObject();
        public int userPackets = 0;
        public int emitPackets = 0;
        public int GameDataEvents = 0;
        public int UpdateSerialization = 0;
        public JSONNode this[string index] {
            get => data[index];
            set => data[index] = value;
        }
        public List<String> groups = new List<string>();
        public Server server { get; protected set; }
        byte[] buffer;
        Dictionary<long, object> Events = new Dictionary<long, object>();

        public User()
        {
            SocketClosed = true;
        }

        public User(CUserToken token, Server server) {
            Logger.Log("Client accepted");
            clientId = ClientIdCounter++;
            this.server = server;
            this.token = token;
            this.token.set_peer(this);
            this.token.disable_auto_heartbeat();
            #region old
            //buffer = new byte[server.bufferSize];

            //try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
            //catch (Exception err) { /* TODO, make a dynamic class for logger (to autoswitch from TXServer.Logger and TXDatabase.Logger)*/ Console.WriteLine(err.ToString(), "Net"); }
            //on<HeartBeat>((HeartBeat packet) => lastResponsesPingId = packet.id);
            //heartBeat = new Timer(
            //    what => {
            //        //if (pingId == lastResponsesPingId)
            //            emit(new HeartBeat(){ id = ++pingId });
            //        //else server.Close(this);
            //    },
            //    null,
            //    10000,
            //    10000
            //);
            #endregion
        }

        void IPeer.on_message(CPacket msg)
        {
            msg.pop_protocol_id();
            OnReceive(msg.pop_bytepack());
        }

        void IPeer.on_removed()
        {
            server.Close(this);
        }

        void IPeer.send(CPacket msg)
        {
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
        }

        void IPeer.disconnect()
        {
            this.token.ban();
            //Close();
        }

        void OnReceive(byte[] newBuffer) {
            //byte[] newBuffer = null;
            
            if (newBuffer.Length == 0) {
                server.Close(this);
                return;
            }

            //Array.Copy(buffer, newBuffer, newBuffer.Length);
            userPackets++;
            SocketClosed = false;
            NetReader reader = new NetReader(newBuffer);

            if (reader.length > Networking.server.bufferSize)
            {
                //lock(receiveLocker)
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
                        //try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
                        //catch { return; }
                        return;
                    }
                }
            }


            Type eventType = HashCache.GetType(reader.hashCode);
            if (eventType == null) {
                Console.WriteLine($"ERR: Unknown event code [hash: {reader.hashCode}]");
                //try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
                //catch { return; }
                return;
            }
            if (!Events.ContainsKey(reader.hashCode)) {
                Console.WriteLine($"ERR: Got packet with no callback [name: {eventType.Name}, hash: {reader.hashCode}]");
                //try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
                //catch { return; }
                return;
            }

            dynamic packetInstance = Activator.CreateInstance(eventType);
            packetInstance.Deserialize(reader);
            
            object eventHandler = Events[reader.hashCode];
            Type callbackType = eventHandler.GetType();
            if (callbackType == typeof(Action))
                ((Action)eventHandler)();
            else if (callbackType == typeof(Action<User>))
                ((Action<User>)eventHandler)(this);
            else if (callbackType == typeof(Action<object>))
                ((Action<object>)eventHandler)(packetInstance);
            else if (callbackType == typeof(Action<User, object>))
                ((Action<User, object>)eventHandler)(this, packetInstance);
            else Console.WriteLine($"Got unknown event callback for packet! [name: {eventType.Name}, hash: {reader.hashCode}, callback: {eventHandler}]");
        }

        private void typedEmit(INetSerializable packet, NetWriter writer)
        {
            emitPackets++;
            var byteBuffer = new List<byte>(writer.ToByteArray());
            int position = 0;
            if (SocketClosed)
                return;
            //Logger.Log("send data " + packet.GetType().ToString() + " " + byteBuffer.Count.ToString());
            while ((((float)byteBuffer.Count) / ((float)Networking.server.bufferSize)) - position - 1 > 0)
            {
                //lock(sendLocker)
                {
                    try
                    {
                        CPacket cPacket = CPacket.create((short)PROTOCOL.Server);
                        cPacket.push(byteBuffer.GetRange(position * Networking.server.bufferSize, Networking.server.bufferSize).ToArray());
                        this.token.send(cPacket);
                        errorCount = 0;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("SocketEmitError: " + ex.StackTrace);
                        errorCount++;
                    }
                }

                position++;
            }
            //lock (sendLocker)
            {
                try
                {
                    CPacket cPacket = CPacket.create((short)PROTOCOL.Server);
                    cPacket.push(byteBuffer.GetRange(position * Networking.server.bufferSize, byteBuffer.Count - position * Networking.server.bufferSize).ToArray());
                    this.token.send(cPacket);
                    errorCount = 0;
                }
                catch (Exception ex)
                {
                    Logger.Log("SocketEmitError: " + ex.StackTrace);
                    errorCount++;
                }
            }
            if (packet.GetType() == typeof(GameDataEvent))
                GameDataEvents++;
            if (errorCount > 100)
                SocketClosed = true;
        }

        public void emit<T>(T packet) where T : struct, INetSerializable {
            NetWriter writer = new NetWriter(HashCache.Get<T>());
            packet.Serialize(writer);
            typedEmit(packet, writer);
        }

        public void emit(INetSerializable packet)
        {
            NetWriter writer = new NetWriter(HashCache.Get(packet.GetType()));
            packet.Serialize(writer);
            typedEmit(packet, writer);
        }

        public bool eventExists<T>() where T : struct, INetSerializable
            => Events.ContainsKey(HashCache.Get<T>());

        public void on<T>(Action callback) where T : struct, INetSerializable {
            if (callback == null) return;

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }

        public void on<T>(Action<User> callback) where T : struct, INetSerializable {
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
        
        public void on<T>(Action<User, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => callback(this, (T)o));
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => callback(this, (T)o)));
        }

        public void onOnce<T>(Action callback) where T : struct, INetSerializable {
            if (callback == null) return;

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }

        public void onOnce<T>(Action<User> callback) where T : struct, INetSerializable {
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
        
        public void onOnce<T>(Action<User, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); });
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); }));
        }

        public void removeEvent<T>() where T : struct, INetSerializable {
            if (eventExists<T>())
                Events.Remove(HashCache.Get<T>());
        }
    
        public void Close() => server.Close(this);
    }
}
