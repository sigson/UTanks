using Core;
using UTanksServer.Network.NetworkEvents.Security;
using UTanksServer.Network.Simple.Net;
using UTanksServer.Network.Simple.Net.Server;
using SimpleJSON;
using System.Net;
using UTanksServer.Database.Databases;
using UTanksServer.Network.NetworkEvents.Communications;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.Communications;
using UTanksServer.Network.NetworkEvents.PlayerAuth;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.PlayerAuth;
using System;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Components.User;
using System.Linq;
using UTanksServer.ECS.Systems.User;
using System.Threading.Tasks;

namespace UTanksServer.Database {
    public static class Authenticator {
        static JSONNode Config { get => Program.Config["Authenticator"]; }
        public static void Process(User user) {
            Logger.LogNetwork($"New connection from assigned id '{user.clientId}'", "Auth");
            //Logger.LogNetwork($"New connection from '{(user.token.socket.RemoteEndPoint as IPEndPoint).Address}' assigned id '{user.clientId}'", "Auth");

            user.on<StartCommunication>(async (StartCommunication packet) => {
                user.emit<StartCommunication>(new StartCommunication() { });
            });

            user.on<LoginEvent>(async (LoginEvent packet) => {
                string APIKey = packet.login;
                string APIToken = packet.password;

                // Find in DB and search 
                UserRow data = (await UserDatabase.Users.LoginCheck(packet.login, HashUtil.MD5(packet.password))? await UserDatabase.Users.GetUserViaCallsign(packet.login) : UserRow.Empty);
                if (data == UserRow.Empty)
                {
                    Logger.LogNetwork($"User '{user.clientId}' sent an invalid data '{packet.login}'", "UserAuth");
                    user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid API key" : string.Empty });
                    return;
                }
                Logger.LogNetwork($"User '{user.clientId}' attempting to login as '{packet.login}'", "Auth");

                // Passed all checks... so allow access
                // cleanup
                user.removeEvent<LoginEvent>();
                user.removeEvent<RegisterUserRequest>();
                // /cleanup
                user["uid"] = user.clientId;
                user["friendlyName"] = packet.login;
                user.PlayerEntityId = Guid.NewGuid().GuidToLong();

                var playerList = ManagerScope.systemManager.SystemsInterestedEntityDatabase.Where(x => x.Key.GetType() == typeof(UserUpdaterSystem)).ToList()[0].Value.Keys.ToList();
                foreach (var serverPlayer in playerList)
                {
                    var playerEnt = ManagerScope.entityManager.EntityStorage[serverPlayer];
                    if (playerEnt.GetComponent<UsernameComponent>().Username == packet.login)
                    {
                        user.PlayerEntityId = playerEnt.instanceId;
                        break;
                    }
                }

                user.emit<UserLoggedInEvent>(new UserLoggedInEvent() { uid = user.clientId, Username = packet.login, entityId = user.PlayerEntityId, serverTime = DateTime.Now.Ticks });
                Lobby.AddUser(user);
            });
            user.on(async (CheckConfigVersion checkConfigEvent) =>
            {
                if (checkConfigEvent.hash == Program.hashConfigFilesZip)
                {
                    user.emit(new CheckConfigResult { uid = Guid.NewGuid().GuidToLong(), newConfig = new System.Collections.Generic.List<byte>(), hash = checkConfigEvent.hash });
                }
                else
                {
                    user.emit(new CheckConfigResult { uid = Guid.NewGuid().GuidToLong(), newConfig = Program.ConfigFilesZip, hash = Program.hashConfigFilesZip });
                }
            });
            user.on(async (RegisterUserRequest packet) => {
                if (Config["RegistrationOpen"])
                    return;
                string username = packet.Username;
                if (!await UserDatabase.Users.UsernameAvailable(packet.Username))
                {
                    user.emit<RegisterUserFailed>(new RegisterUserFailed { reason = "Such username already exists" });
                    return;
                }
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' creating user '{username}'", "DBAccess");
                UserRow data;
                try
                {
                    data = await UserDatabase.Users.Create(
                        username,
                        HashUtil.MD5(packet.Password),
                        packet.Email,
                        packet.HardwareId,
                        packet.HardwareToken
                    );
                }
                catch(Exception ex)
                {
                    user.emit<RegisterUserFailed>(new RegisterUserFailed { reason = ex.Message });
                    return;
                }

                Logger.LogDebug(data.ToString());
                //user.emit(new AvailableResult { packetId = packet.packetId, result = true });

                user.removeEvent<LoginEvent>();
                user.removeEvent<RegisterUserRequest>();

                // /cleanup
                user["uid"] = user.clientId;
                user["friendlyName"] = packet.Username;
                user.PlayerEntityId = Guid.NewGuid().GuidToLong();

                var playerList = ManagerScope.systemManager.SystemsInterestedEntityDatabase.Where(x => x.Key.GetType() == typeof(UserUpdaterSystem)).ToList()[0].Value.Keys.ToList();
                foreach (var serverPlayer in playerList)
                {
                    var playerEnt = ManagerScope.entityManager.EntityStorage[serverPlayer];
                    if (playerEnt.GetComponent<UsernameComponent>().Username == packet.Username)
                    {
                        user.PlayerEntityId = playerEnt.instanceId;
                        break;
                    }
                }

                user.emit<UserLoggedInEvent>(new UserLoggedInEvent() { uid = user.clientId, Username = packet.Username, entityId = user.PlayerEntityId, serverTime = DateTime.Now.Ticks });
                Lobby.AddUser(user);
            });
        }
    }
    #region oldAuth
    //public static class Authenticator
    //{
    //    static JSONNode Config { get => Program.Config["Authenticator"]; }
    //    public static void Process(User user)
    //    {
    //        Logger.LogNetwork($"New connection from '{(user.socket.RemoteEndPoint as IPEndPoint).Address}' assigned id '{user.clientId}'", "Auth");
    //        user.emit<RSAPublicKey>(new RSAPublicKey() { Key = user.RSADecryptionComponent.publicKey });
    //        user.onOnce<RSAPublicKey>((RSAPublicKey packet) => {
    //            user.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key);
    //            user.on<RSAPublicKey>((RSAPublicKey packet)
    //                => user.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key)
    //            );

    //            user.on<LoginEvent>(async (LoginEvent packet) => {
    //                string APIKey = user.RSADecryptionComponent.DecryptToString(packet.login);
    //                string APIToken = user.RSADecryptionComponent.DecryptToString(packet.password);

    //                // Find in DB and search 
    //                ServerRow data = await ServerDatabase.Servers.Get(HashUtil.MD5(APIKey));
    //                if (data == ServerRow.Empty)
    //                {
    //                    Logger.LogNetwork($"User '{user.clientId}' sent an invalid key '{data.name}'", "Auth");
    //                    user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid API key" : string.Empty });
    //                    return;
    //                }
    //                Logger.LogNetwork($"User '{user.clientId}' attempting to login as '{data.name}'", "Auth");
    //                HashUtilCheckResult result = HashUtil.Check(data.token, APIToken);
    //                if (!result.verified)
    //                {
    //                    Logger.LogNetwork($"User '{user.clientId}' failed to login as '{data.name}', token error", "Auth");
    //                    user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid API token" : string.Empty });
    //                    return;
    //                }
    //                if (result.needsUpgrade)
    //                {
    //                    Logger.Log($"Server '{data.name}' API Token has been upgraded", "Net");
    //                    data.token = HashUtil.Compute(APIKey);
    //                    await ServerDatabase.Servers.Save(data);
    //                }
    //                if (!data.addresses.Contains($"{(user.socket.RemoteEndPoint as IPEndPoint).Address}"))
    //                {
    //                    Logger.LogNetwork($"User '{user.clientId}' failed to login as '{data.name}', IP Check failed", "Auth");
    //                    user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid remote endpoint" : string.Empty });
    //                    return;
    //                }

    //                // Passed all checks... so allow access
    //                // cleanup
    //                user.removeEvent<LoginEvent>();
    //                // /cleanup
    //                user["uid"] = data.uid;
    //                user["friendlyName"] = data.name;
    //                user.emit<LoginSuccessEvent>(new LoginSuccessEvent());
    //                Lobby.AddUser(user);
    //            });
    //        });
    //    }
    //}
    #endregion
}
