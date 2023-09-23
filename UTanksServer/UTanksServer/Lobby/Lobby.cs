using UTanksServer.Network.Simple.Net;
using UTanksServer.Network.Simple.Net.Server;
using Core;
using UTanksServer.Network.NetworkEvents.Communications;
using UTanksServer.Network.NetworkEvents.PlayerAuth;
using UTanksServer.Network.NetworkEvents.PlayerSettings;
using UTanksServer.Database.Databases;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.PlayerAuth;
using SimpleJSON;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using UTanksServer.ECS.ECSCore;
using System;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UTanksServer.ECS.Templates.User;
using UTanksServer.ECS.Events.User;
using System.IO;
using UTanksServer.Network.NetworkEvents.FastGameEvents;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.ECS.Events.Battle.BonusEvents;
using UTanksServer.ECS.Events.Battle.TankEvents.Shooting;
using System.Linq;
using UTanksServer.ECS.Systems.User;
using UTanksServer.ECS.Components.User;

namespace UTanksServer.Database {
    public static class Lobby {
        static Server server { get => Networking.server; }
        const string authenticatedGroupName = "authComplete";
        public static JSONNode Config => Program.Config["JTServer"];

        public static void AddUser(User user) {
            user.groups.Add(authenticatedGroupName);

            Logger.LogNetwork($"User '{user.clientId}' logged in as '{(string)user.data["friendlyName"]}'", "Auth");
            
            user.on((UserLoggedInEvent packet) => {
                Logger.LogNetwork($"Player (of uid) '{packet.uid}' has logged into '{(string)user["friendlyName"]}'", "Broadcast");
                server.broadcastExcept(authenticatedGroupName, packet, user.clientId);
            });
            user.on(async (GetUserViaUsername packet) => {
                if(user["friendlyName"] == packet.Username)
                {
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}' accessing user");
                    UserRow data = await UserDatabase.Users.GetUserViaCallsign(packet.Username);
                    if (data == UserRow.Empty)
                    {
                        Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{packet.Username}' not found", "DBAccess");
                        user.emit(new UserInitialDataEvent()
                        {
                            packetId = packet.packetId,
                            uid = -1
                        });
                    }
                    else
                    {
                        Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{packet.Username}' found", "DBAccess");
                        //user.emit();
                        
                        Func<Task> asyncECSLogin = async () =>
                        {
                            await Task.Run(() => {
                                ECSEntity playerEnt = null;
                                if(ManagerScope.entityManager.EntityStorage.TryGetValue(user.PlayerEntityId, out playerEnt))
                                {
                                    ManagerScope.eventManager.OnEventAdd(new UserLogged()
                                    {
                                        EntityOwnerId = playerEnt.instanceId,
                                        instanceId = Guid.NewGuid().GuidToLong(),
                                        networkSocket = user,
                                        userEntity = playerEnt,
                                        userRelogin = true
                                    });
                                }
                                else
                                {
                                    var entityUser = new UserTemplate().CreateEntity(data, user.PlayerEntityId);
                                    #region biggy
                                    //new UserInitialDataEvent()
                                    //{
                                    //    packetId = packet.packetId,
                                    //    uid = data.id,
                                    //    Username = data.Username,
                                    //    Password = data.Password,
                                    //    Email = data.Email,
                                    //    EmailVerified = data.EmailVerified,
                                    //    Clan = data.Clan,
                                    //    Crystalls = data.Crystalls,
                                    //    Rank = data.Rank,
                                    //    GlobalScore = data.GlobalScore,
                                    //    RankScore = data.RankScore,
                                    //    Karma = data.Karma,
                                    //    TermlessBan = data.TermlessBan,
                                    //    HardwareId = data.HardwareId,
                                    //    HardwareToken = data.HardwareToken,
                                    //    ActiveChatBanEndTime = data.ActiveChatBanEndTime,
                                    //    GarageJSONData = data.GarageJSONData,
                                    //    LastDatetimeGetDailyBonus = data.LastDatetimeGetDailyBonus,
                                    //    LastIp = data.LastIp,
                                    //    RegistrationDate = data.RegistrationDate,
                                    //    UserGroup = data.UserGroup,
                                    //    UserLocation = data.UserLocation
                                    //}
                                    #endregion
                                    ManagerScope.eventManager.OnEventAdd(new UserLogged()
                                    {
                                        EntityOwnerId = entityUser.instanceId,
                                        instanceId = Guid.NewGuid().GuidToLong(),
                                        networkSocket = user,
                                        userEntity = entityUser,
                                        userRelogin = false
                                    });
                                }
                                
                            });
                        };
                        asyncECSLogin();
                    }
                }
                
            });
            user.on(async (UsernameAvailableRequest packet) => {
                string username = user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername);
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if '{username}' exists", "DBAccess");
                AvailableResult response = new AvailableResult()
                {
                    packetId = packet.packetId,
                    result = await UserDatabase.Users.UsernameAvailable(username)
                };
                if (response.result)
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' exists", "DBAccess");
                else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' does not exist", "DBAccess");
                user.emit(response);
            });
            user.on(async (EmailAvailableRequest packet) => {
                string username = user.RSADecryptionComponent.DecryptToString(packet.email);
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if email '{username}' is availables", "DBAccess");
                AvailableResult response = new AvailableResult()
                {
                    packetId = packet.packetId,
                    result = await UserDatabase.Users.EmailAvailable(username)
                };
                if (response.result)
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is available", "DBAccess");
                else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is not available", "DBAccess");
                user.emit(response);
            });
            user.on(async (GameDataEvent gameDataEvent) =>
            {
                Type eventType;
                //if(ManagerScope.eventManager.AcceptedOutsideEvents.Contains(gameDataEvent.typeId))
                {
                    if (ManagerScope.eventManager.EventSerializationCache.TryGetValue(gameDataEvent.typeId, out eventType))
                    {
                        Func<Task> asyncSend = async () =>
                        {
                            await Task.Run(() => {
                                StringReader reader = new StringReader(gameDataEvent.jsonData);
                                var unserializedEvent = (ECSEvent)GlobalCachingSerialization.standartSerializer.Deserialize(reader, eventType);
                                reader.Close();
                                unserializedEvent.EntityOwnerId = user.PlayerEntityId;
                                unserializedEvent.cachedGameDataEvent = gameDataEvent;
                                unserializedEvent.cachedRawEvent = gameDataEvent;
                                ManagerScope.eventManager.OnEventAdd(unserializedEvent);
                            });
                        };
                        asyncSend();
                    }
                }
                
            });
            

            user.on(async (RawMovementEvent rawMovementEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawMovementEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new MoveCommandEvent() 
                        {
                            cachedRawEvent = rawMovementEvent,
                            EntityOwnerId = user.PlayerEntityId,
                            ClientTime = rawMovementEvent.ClientTime,
                            position = rawMovementEvent.position,
                            rotation = rawMovementEvent.rotation,
                            TankMoveControl = rawMovementEvent.TankMoveControl,
                            TankTurnControl = rawMovementEvent.TankTurnControl,
                            turretRotation = rawMovementEvent.turretRotation,
                            velocity = rawMovementEvent.velocity,
                            angularVelocity = rawMovementEvent.angularVelocity,
                            WeaponRotation = rawMovementEvent.WeaponRotation,
                            WeaponRotationControl = rawMovementEvent.WeaponRotationControl
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawDropTakingEvent rawDropTakingEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {

                    await Task.Run(() => {
                        rawDropTakingEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new BonusTakingEvent()
                        {
                            cachedRawEvent = rawDropTakingEvent,
                            EntityOwnerId = user.PlayerEntityId,
                            DropId = rawDropTakingEvent.dropEntityId,
                            contactPosition = rawDropTakingEvent.contactPosition
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawHitEvent rawHitEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawHitEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new HitEvent()
                        {
                            cachedRawEvent = rawHitEvent,
                            EntityOwnerId = user.PlayerEntityId,
                            hitDistanceList = rawHitEvent.hitDistanceList,
                            hitList = rawHitEvent.hitList,
                            hitLocalDistanceList = rawHitEvent.hitLocalDistanceList
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawShotEvent rawShotEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawShotEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new ShotEvent()
                        {
                            cachedRawEvent = rawShotEvent,
                            EntityOwnerId = user.PlayerEntityId,
                            hitDistanceList = rawShotEvent.hitDistanceList,
                            hitList = rawShotEvent.hitList,
                            MoveDirectionNormalized = rawShotEvent.MoveDirectionNormalized,
                            StartGlobalPosition = rawShotEvent.StartGlobalPosition,
                            StartGlobalRotation = rawShotEvent.StartGlobalRotation,
                            hitLocalDistanceList = rawShotEvent.hitLocalDistanceList
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawStartShootingEvent rawStartShootingEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawStartShootingEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new StartShootingEvent()
                        {
                            cachedRawEvent = rawStartShootingEvent,
                            EntityOwnerId = user.PlayerEntityId
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawEndShootingEvent rawStopShootingEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawStopShootingEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new EndShootingEvent()
                        {
                            cachedRawEvent = rawStopShootingEvent,
                            EntityOwnerId = user.PlayerEntityId
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawSupplyUsingEvent rawSupplyUsingEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        rawSupplyUsingEvent.PlayerEntityId = user.PlayerEntityId;
                        ManagerScope.eventManager.OnEventAdd(new SupplyUsedEvent()
                        {
                            cachedRawEvent = rawSupplyUsingEvent,
                            targetEntities = rawSupplyUsingEvent.targetEntities,
                            EntityOwnerId = user.PlayerEntityId,
                            supplyPath = rawSupplyUsingEvent.supplyPath
                        });
                    });
                };
                asyncSend();

            });
            user.on(async (RawCreatureActuationEvent rawCreatureActuationEvent) =>
            {
                Func<Task> asyncSend = async () =>
                {
                    await Task.Run(() => {
                        ManagerScope.eventManager.OnEventAdd(new CreatureActuationEvent()
                        {
                            cachedRawEvent = rawCreatureActuationEvent,
                            EntityOwnerId = user.PlayerEntityId,
                            CreatureInstanceId = rawCreatureActuationEvent.CreatureInstanceId,
                            BattleDBOwnerId = rawCreatureActuationEvent.BattleDBOwnerId,
                            TargetsId = rawCreatureActuationEvent.TargetsId
                        });
                    });
                };
                asyncSend();

            });

            #region oldEvents
            //user.on(async (GetUserSettingsRequest request) =>
            //{
            //    Logger.LogNetwork($"'{(string)user["friendlyName"]}' getting user settings for uid '{request.uid}'", "DBAccess");
            //    UserSettingsData result = await UserDatabase.UserSettings.Get(request.uid);
            //    result.packetId = request.packetId;
            //    result.countryCode = user.RSAEncryptionComponent.Encrypt(result.countryCode);
            //    result.avatar = user.RSAEncryptionComponent.Encrypt(result.avatar);
            //    user.emit(result);
            //});
            //
            // Player account setters
            //user.on(async (SetUsername request)
            //    => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing username of '{request.uid}'. Success: {(await UserDatabase.Users.SetUsername(request.uid, user.RSADecryptionComponent.DecryptToString(request.username)) ? "TRUE" : "FALSE")}", "DBAccess"));
            //user.on(async (SetHashedPassword request)
            //    => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing password of '{request.uid}'. Success: {(await UserDatabase.Users.SetHashedPassword(request.uid, user.RSADecryptionComponent.DecryptToString(request.hashedPassword)) ? "TRUE" : "FALSE")}", "DBAccess"));
            //user.on(async (SetEmail request)
            //    => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing email of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmail(request.uid, user.RSADecryptionComponent.DecryptToString(request.email)) ? "TRUE" : "FALSE")}", "DBAccess"));
            //user.on(async (SetEmailVerified request)
            //    => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing setting email verified state of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmailVerified(request.uid, request.state) ? "TRUE" : "FALSE")}", "DBAccess"));
            #endregion
        }

        public static void RemoveUser(User user) { 
            if (user.groups.Contains(authenticatedGroupName))
                Logger.LogNetwork($"User '{user.clientId}' (aka '{(string)user["friendlyName"]}') has disconnected");
            else Logger.LogNetwork($"User '{user.clientId}' (unauthenticated) has disconnected");
        }
    }
    #region oldlobby
    //public static class Lobby
    //{
    //    static Server server { get => Networking.server; }
    //    const string authenticatedGroupName = "authComplete";
    //    public static void AddUser(User user)
    //    {
    //        user.groups.Add(authenticatedGroupName);

    //        Logger.LogNetwork($"User '{user.clientId}' logged in as '{(string)user.data["friendlyName"]}'", "Auth");

    //        user.on((UserLoggedInEvent packet) => {
    //            Logger.LogNetwork($"Player (of uid) '{packet.uid}' has logged into '{(string)user["friendlyName"]}'", "Broadcast");
    //            server.broadcastExcept(authenticatedGroupName, packet, user.clientId);
    //        });

    //        user.on(async (RegisterUserRequest packet) => {
    //            string username = user.RSADecryptionComponent.DecryptToString(packet.Username);
    //            Logger.LogNetwork($"'{(string)user["friendlyName"]}' creating user '{username}'", "DBAccess");
    //            UserRow data = await UserDatabase.Users.Create(
    //                username,
    //                user.RSADecryptionComponent.DecryptToString(packet.Password),
    //                user.RSADecryptionComponent.DecryptToString(packet.Email),
    //                user.RSADecryptionComponent.DecryptToString(packet.HardwareId),
    //                user.RSADecryptionComponent.DecryptToString(packet.HardwareToken)
    //            );
    //            _ = await UserDatabase.UserSettings.Get(data.id); // Get will just make sure that the row exists (it will create a row if one for the uid does not exist, HEY, I am lazy)
    //            _ = await UserDatabase.UserSettings.SetCountryCode(data.id, user.RSADecryptionComponent.DecryptToString(packet.CountryCode));
    //            _ = await UserDatabase.UserSettings.SetSubscribedState(data.id, packet.subscribed);
    //            /* Shouldn't happen
    //            if (data == UserRow.Empty) {
    //                Logger.Log($"'{(string)user["friendlyName"]}' failed to create user '{username}'", "DBAccess");
    //            }*/
    //            Logger.LogDebug(data.ToString());
    //            user.emit(new UserInitialDataEvent()
    //            {
    //                packetId = packet.packetId,
    //                uid = data.id,
    //                username = user.RSAEncryptionComponent.Encrypt(data.Username),
    //                hashedPassword = user.RSAEncryptionComponent.Encrypt(data.Password),
    //                email = user.RSAEncryptionComponent.Encrypt(data.Email),
    //                emailVerified = data.EmailVerified,
    //                hardwareId = user.RSAEncryptionComponent.Encrypt(data.HardwareId),
    //                hardwareToken = user.RSAEncryptionComponent.Encrypt(data.HardwareToken)
    //            });
    //        });

    //        user.on(async (GetUserViaUsername packet) => {
    //            Logger.LogNetwork($"'{(string)user["friendlyName"]}' accessing user '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' auth data", "DBAccess");
    //            UserRow data = await UserDatabase.Users.GetUserViaCallsign(user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername));
    //            if (data == UserRow.Empty)
    //            {
    //                Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' not found", "DBAccess");
    //                user.emit(new UserInitialDataEvent()
    //                {
    //                    packetId = packet.packetId,
    //                    uid = -1
    //                });
    //            }
    //            else
    //            {
    //                Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' found", "DBAccess");
    //                user.emit(new UserInitialDataEvent()
    //                {
    //                    packetId = packet.packetId,
    //                    uid = data.id,
    //                    username = user.RSAEncryptionComponent.Encrypt(data.Username),
    //                    hashedPassword = user.RSAEncryptionComponent.Encrypt(data.Password),
    //                    email = user.RSAEncryptionComponent.Encrypt(data.Email),
    //                    emailVerified = data.EmailVerified,
    //                    hardwareId = user.RSAEncryptionComponent.Encrypt(data.HardwareId),
    //                    hardwareToken = user.RSAEncryptionComponent.Encrypt(data.HardwareToken)
    //                });
    //            }
    //        });
    //        user.on(async (UsernameAvailableRequest packet) => {
    //            string username = user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername);
    //            Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if '{username}' exists", "DBAccess");
    //            AvailableResult response = new AvailableResult()
    //            {
    //                packetId = packet.packetId,
    //                result = await UserDatabase.Users.UsernameAvailable(username)
    //            };
    //            if (response.result)
    //                Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' exists", "DBAccess");
    //            else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' does not exist", "DBAccess");
    //            user.emit(response);
    //        });
    //        user.on(async (EmailAvailableRequest packet) => {
    //            string username = user.RSADecryptionComponent.DecryptToString(packet.email);
    //            Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if email '{username}' is availables", "DBAccess");
    //            AvailableResult response = new AvailableResult()
    //            {
    //                packetId = packet.packetId,
    //                result = await UserDatabase.Users.EmailAvailable(username)
    //            };
    //            if (response.result)
    //                Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is available", "DBAccess");
    //            else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is not available", "DBAccess");
    //            user.emit(response);
    //        });

    //        user.on(async (GetUserSettingsRequest request) =>
    //        {
    //            Logger.LogNetwork($"'{(string)user["friendlyName"]}' getting user settings for uid '{request.uid}'", "DBAccess");
    //            UserSettingsData result = await UserDatabase.UserSettings.Get(request.uid);
    //            result.packetId = request.packetId;
    //            result.countryCode = user.RSAEncryptionComponent.Encrypt(result.countryCode);
    //            result.avatar = user.RSAEncryptionComponent.Encrypt(result.avatar);
    //            user.emit(result);
    //        });

    //        // Player account setters
    //        user.on(async (SetUsername request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing username of '{request.uid}'. Success: {(await UserDatabase.Users.SetUsername(request.uid, user.RSADecryptionComponent.DecryptToString(request.username)) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetHashedPassword request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing password of '{request.uid}'. Success: {(await UserDatabase.Users.SetHashedPassword(request.uid, user.RSADecryptionComponent.DecryptToString(request.hashedPassword)) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetEmail request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing email of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmail(request.uid, user.RSADecryptionComponent.DecryptToString(request.email)) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetEmailVerified request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing setting email verified state of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmailVerified(request.uid, request.state) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetUserRememberMeCredentials request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' set AutoLogin parameters for '{request.uid}'. Success: {(await UserDatabase.Users.SetRememberMe(request.uid, user.RSADecryptionComponent.DecryptToString(request.hardwareId), user.RSADecryptionComponent.DecryptToString(request.hardwareToken)) ? "TRUE" : "FALSE")}", "DBAccess"));

    //        // Player settings setters
    //        user.on(async (SetCountryCode request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing country code of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetCountryCode(request.uid, user.RSADecryptionComponent.DecryptToString(request.countryCode)) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetAvatar request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing avatar of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetAvatar(request.uid, user.RSADecryptionComponent.DecryptToString(request.avatar)) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetPremiumExpiration request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing premium expiration for '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetPremiumExpiration(request.uid, request.expiration) ? "TRUE" : "FALSE")}", "DBAccess"));
    //        user.on(async (SetSubscribed request)
    //            => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing country code of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetSubscribedState(request.uid, request.state) ? "TRUE" : "FALSE")}", "DBAccess"));
    //    }

    //    public static void RemoveUser(User user)
    //    {
    //        if (user.groups.Contains(authenticatedGroupName))
    //            Logger.LogNetwork($"User '{user.clientId}' (aka '{(string)user["friendlyName"]}') has disconnected");
    //        else Logger.LogNetwork($"User '{user.clientId}' (unauthenticated) has disconnected");
    //    }
    //}
    #endregion
}
