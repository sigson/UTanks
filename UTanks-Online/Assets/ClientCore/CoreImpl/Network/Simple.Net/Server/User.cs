using System;
using System.Threading;
//using System.Timers;
using System.Collections.Generic;
using System.Net.Sockets;
using SimpleJSON;
using UTanksClient.Network.Simple.Net.InternalEvents;
using System.Collections.Concurrent;
using UTanksClient.ClassExtensions;
using UTanksClient.Extensions;

namespace UTanksClient.Network.Simple.Net.Server {
    public class User {
        public int clientId { get; protected set; }
        static int ClientIdCounter = 0;
        public Timer heartBeat;
        long pingId = 0;
        long lastResponsesPingId = 0;

        public RSADecryptComponent RSADecryptionComponent = new RSADecryptComponent();
        public RSAEncryptCompoenent RSAEncryptionComponent; // Defined in Authenticator.Process => user.on<RSAPublicKey>
        ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>> SlicedPackets = new ConcurrentDictionary<long, ConcurrentDictionary<long, byte[]>>();
        public Socket socket { get; protected set; }
        public JSONNode data = new JSONObject();
        public JSONNode this[string index] {
            get => data[index];
            set => data[index] = value;
        }
        public List<String> groups = new List<string>();
        public Server server { get; protected set; }
        byte[] buffer;
        int BufferSize = 2048;
        Dictionary<long, object> Events = new Dictionary<long, object>();

        public User(Socket socket, Server server) {
            clientId = ClientIdCounter++;
            this.socket = socket;
            this.server = server;
            buffer = new byte[server.bufferSize];

            try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
            catch (Exception err) { /* TODO, make a dynamic class for logger (to autoswitch from TXServer.Logger and TXDatabase.Logger)*/ Console.WriteLine(err.ToString(), "Net"); }
            on<HeartBeat>((HeartBeat packet) => lastResponsesPingId = packet.id);
            this.BufferSize = server.bufferSize;
            heartBeat = new Timer(
                what => {
                    if (pingId == lastResponsesPingId)
                        emit(new HeartBeat(){ id = ++pingId });
                    else server.Close(this);
                },
                null,
                5000,
                5000
            );
        }

        void OnReceive(IAsyncResult asyncResult)
        {
            byte[] newBuffer = null;
            try
            {
                newBuffer = new byte[socket.EndReceive(asyncResult)];
            }
            catch (SocketException)
            {
                try { server.Close(this); } catch { }
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            if (newBuffer.Length == 0)
            {
                server.Close(this);
                return;
            }

            Array.Copy(buffer, newBuffer, newBuffer.Length);


            NetReader reader = new NetReader(newBuffer);

            if (reader.length > newBuffer.Length)
            {
                ConcurrentDictionary<long, byte[]> bufPackets;
                var getResult = SlicedPackets.TryGetValue(reader.id, out bufPackets);
                if (getResult && reader.length - bufPackets.Count * this.BufferSize <= this.BufferSize)
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
                    try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
                    catch { return; }
                    return;
                }
            }

            Type eventType = HashCache.GetType(reader.hashCode);
            if (eventType == null)
            {
                Console.WriteLine($"ERR: Unknown event  code [hash: {reader.hashCode}]");
                return;
            }
            if (!Events.ContainsKey(reader.hashCode))
            {
                Console.WriteLine($"ERR: Got packet with no callback [name: {eventType.Name}, hash: {reader.hashCode}]");
                return;
            }

            object packetInstance = Activator.CreateInstance(eventType);
            (packetInstance as INetSerializable).Deserialize(reader);

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

            try { socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket); }
            catch { return; }
        }

        public void emit<T>(T packet) where T : struct, INetSerializable {
            NetWriter writer = new NetWriter(HashCache.Get<T>());
            packet.Serialize(writer);
            var byteBuffer = new List<byte>(writer.ToByteArray());
            int position = 0;
            while ((((float)byteBuffer.Count) / ((float)ClientNetworkService.instance.Socket.socket.ReceiveBufferSize)) - position - 1f > 0)
            {
                try { socket.Send(byteBuffer.GetRange(position * ClientNetworkService.instance.Socket.socket.ReceiveBufferSize, ClientNetworkService.instance.Socket.socket.ReceiveBufferSize).ToArray()); } catch { }
                position++;
            }
            try { socket.Send(byteBuffer.GetRange(position * ClientNetworkService.instance.Socket.socket.ReceiveBufferSize, byteBuffer.Count - position * ClientNetworkService.instance.Socket.socket.ReceiveBufferSize).ToArray()); } catch { }
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
