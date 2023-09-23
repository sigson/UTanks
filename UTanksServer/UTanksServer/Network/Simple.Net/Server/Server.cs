using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using BitNet;
using UTanksServer.Core.Logging;

namespace UTanksServer.Network.Simple.Net.Server {
    public class Server {

        public int port { get; private set; }
        public Socket socket { get; protected set; }
        public List<User> users { get; private set; } = new List<User>();
        public int bufferSize { get; private set; }
        Action<User> onConnect;
        Action<User> onDisconnect;

        CNetworkService serverService;

        public Server(int bufferSize = 2048)
        {
            this.bufferSize = bufferSize;
        }

        public Server(int port, Action<User> onConnect, Action<User> onDisconnect, int bufferSize = 2048) {
            if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException("Parameter 'port' must be between 1 and 65,535");
            if (onConnect == null || onDisconnect == null) throw new ArgumentNullException("Paremeters 'onConnect' and 'onDisconnect' must both be defined");
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException("Parameter 'bufferSize' must be above 0");

            serverService = new CNetworkService(false);
            serverService.session_created_callback += UserAccepted;
            serverService.initialize(10000, 1024);
            this.port = port;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            this.bufferSize = bufferSize;
        }

        public void Listen() {
            serverService.listen("0.0.0.0", this.port, 100);
            //serverService.listen("127.0.0.1", this.port, 100);
        }

        void UserAccepted(CUserToken token)
        {
            User user;
            try { user = new User(token, this); }
            catch
            {
                Logger.Log("error add user");
                return;
            }
            users.Add(user);
            onConnect(user);
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

            //User user;
            //try { user = new User(client, this); }
            //catch { 
            //    client.Shutdown(SocketShutdown.Both);
            //    client.Close();
            //    return; 
            //}
            //users.Add(user);
            //onConnect(user);
            socket.BeginAccept(OnConnect, null);
        }

        public void Close(User user) {
            if (users.Contains(user)) {
                users.Remove(user);
                onDisconnect(user);
            }
            user.token.close();
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