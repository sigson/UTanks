using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace UTanksClient.Network.Simple.Net.Server {
    public class Server {
        public static int GetRandomUnusedPort()
        {
            TcpListener _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            int port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _listener.Stop();
            return port;
        }

        public int port { get; private set; }
        public Socket socket { get; protected set; }
        public List<User> users { get; private set; } = new List<User>();
        public int bufferSize { get; private set; }

        NetworkStream networkStream;
        Dictionary<long, object> Events = new Dictionary<long, object>();

        Action<User> onConnect;
        Action<User> onDisconnect;

        public Server(int port, Action<User> onConnect, Action<User> onDisconnect, int bufferSize = 2048) {
            if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException("Parameter 'port' must be between 1 and 65,535");
            if (onConnect == null || onDisconnect == null) throw new ArgumentNullException("Paremeters 'onConnect' and 'onDisconnect' must both be defined");
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException("Parameter 'bufferSize' must be above 0");

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.port = port;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            this.bufferSize = bufferSize;
        }

        public void Listen() {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(5);
            
            socket.BeginAccept(OnConnect, null);
        }

        void OnConnect(IAsyncResult asyncResult) {
            Socket client;
            try {
                client = socket.EndAccept(asyncResult);
            }
            catch {
                socket.BeginAccept(OnConnect, null);
                return;
            }

            User user;
            try { user = new User(client, this); }
            catch { 
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                return; 
            }
            users.Add(user);
            onConnect(user);
            socket.BeginAccept(OnConnect, null);
        }

        //void OnReceive(IAsyncResult asyncResult)
        //{
        //    byte[] newBuffer = null;
        //    try
        //    {
        //        newBuffer = new byte[networkStream.EndRead(asyncResult)];
        //    }
        //    catch
        //    {
        //        Disconnect();
        //        return;
        //    }
        //    if (newBuffer.Length == 0)
        //    {
        //        Disconnect();
        //        return;
        //    }

        //    //Array.Copy(buffer, newBuffer, newBuffer.Length);

        //    NetReader reader = new NetReader(newBuffer);
        //    Type eventType = HashCache.GetType(reader.hashCode);
        //    if (eventType == null)
        //    {
        //        Console.WriteLine($"ERR: Unknown event code [hash: {reader.hashCode}]");
        //        return;
        //    }
        //    if (!Events.ContainsKey(reader.hashCode))
        //    {
        //        Console.WriteLine($"ERR: Got packet with no callback [name: {eventType.Name}, hash: {reader.hashCode}]");
        //        return;
        //    }

        //    dynamic packetInstance = Activator.CreateInstance(eventType);
        //    packetInstance.Deserialize(reader);

        //    object eventHandler = Events[reader.hashCode];
        //    Type callbackType = eventHandler.GetType();
        //    if (callbackType == typeof(Action))
        //        ((Action)eventHandler)();
        //    else if (callbackType == typeof(Action<User>))
        //        ((Action<User>)eventHandler)((User)asyncResult.AsyncState);
        //    else if (callbackType == typeof(Action<object>))
        //        ((Action<object>)eventHandler)(packetInstance);
        //    else if (callbackType == typeof(Action<User, object>))
        //        ((Action<User, object>)eventHandler)((User)asyncResult.AsyncState, packetInstance);
        //    else Console.WriteLine($"Got unknown event callback for packet! [name: {eventType.Name}, hash: {reader.hashCode}, callback: {eventHandler}]");

        //    try { networkStream.BeginRead(buffer, 0, buffer.Length, OnReceive, null); }
        //    catch { return; }
        //}

        public void Close(User user) {
            if (users.Contains(user)) {
                users.Remove(user);
                onDisconnect(user);
            }
            user.heartBeat.Dispose();
            user.socket.Shutdown(SocketShutdown.Both);
            user.socket.Close();
        }

        public void broadcast<T>(T packet) where T : struct, INetSerializable {
            foreach (User user in users.ToArray())
                user.emit<T>(packet);
        }

        public void broadcast<T>(string groupName, T packet) where T : struct, INetSerializable {
            foreach (User user in users.ToArray())
                if (user.groups.Contains(groupName))
                    user.emit<T>(packet);
        }

        public void broadcastExcept<T>(T packet, int clientId) where T : struct, INetSerializable{
            foreach (User user in users.ToArray())
                if (clientId != user.clientId)
                    user.emit<T>(packet);
        }

        public void broadcastExcept<T>(string groupName, T packet, int clientId) where T : struct, INetSerializable {
            foreach (User user in users.ToArray())
                if (user.groups.Contains(groupName) && clientId != user.clientId)
                    user.emit<T>(packet);
        }
    }
}