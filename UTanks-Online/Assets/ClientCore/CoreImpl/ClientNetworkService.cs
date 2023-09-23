using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.Network.NetworkEvents.Security;
using UTanksClient.Network.Simple.Net;
using UTanksClient.Network.Simple.Net.Client;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.Communications;
using UTanksClient.Network.NetworkEvents.Communications;
using UTanksClient.Network.NetworkEvents.PlayerAuth;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.PlayerAuth;
using UnityEngine;
using ULogger = UTanksClient.Core.Logging.ULogger;
using System.Collections;
using System.Threading;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using UTanksClient.ECS.ECSCore;
using System.IO;
//using Assets.ClientCore.CoreImpl.ECS.Templates;
using UTanksClient.ECS.Templates.User;
using UTanksClient.Network.NetworkEvents.FastGameEvents;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Events.Battle.BonusEvents;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.Network.Simple.Net.InternalEvents;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.ECSComponentsGroup;
using UTanksClient.ECS.Components.Battle.Energy;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.FastGameEvents;
using UTanksClient.ECS.Events.ECSEvents;
using UTanksClient.Services;
using Assets.ClientCore.CoreImpl.ECS.Events.Battle.TankEvents.Shooting;
using SecuredSpace.UI.GameUI;
using UTanksClient.Extensions;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Tank;
using UTanksClient.ECS.Components;

namespace UTanksClient
{
    public class ClientNetworkService : IService
    {
        public static ClientNetworkService instance => SGT.Get<ClientNetworkService>();
        [NonSerialized]public Client Socket;
        [NonSerialized] public bool AuthError;
        [NonSerialized] public string AuthErrorReason;
        public string Username;
        public long PlayerEntityId = 0;
        public long ConnectionId = 0;
        public bool LoggedInGame;
        public bool Connected = false;
        public bool ServerAvailable = false;
        [NonSerialized] public Action<CaptchaRequiredEvent> CaptchaRequired;
        [NonSerialized] public Action OnServerAvailableAction = null;
        [NonSerialized] public (string, string) lastSendedAutentificationCredentials;
        JSONNode Config { get => SGT.getInstance<ClientInitService>().Config["Network"]; }
        public bool ready { get; private set; }

        public void InitConnection()
        {
            //ULogger.LogDebug("Opening new instance (DatabaseNetwork)");
            Socket = new Client(Config["HostAddress"].Value,
                                int.Parse(Config["HostPort"].Value),
                                () => { OnConnect(); },
                                () =>
                                {
                                    ULogger.LogDebug("Server disconnected");
                                    OnDisconnect();
                                }, Config["BufferSize"]);
            Socket.Connect();
        }

        public void OnDisconnect()
        {
            Connected = false;
            Socket.Connect();
            TaskEx.RunAsync(() =>
            {
                while (!Connected)
                {
                    //Socket.connector.disconnect();
                    Socket.Connect();
                    Task.Delay(4000).Wait();
                }
            });
        }

        public void OnConnect()
        {
            Connected = true;
            ULogger.LogLoad("1/5/Connected to server...");
            Socket.on<StartCommunication>(OnServerAvailable);
            //Thread.Sleep(7000);
            Socket.emit<StartCommunication>(new StartCommunication());
        }
        public void OnServerAvailable()
        {
            ULogger.LogLoad("2/5/Wait server...");
            
            Socket.on<UserLoggedInEvent>(packet =>
            {
                ConnectionId = packet.uid;
                Username = packet.Username;
                PlayerEntityId = packet.entityId;
                LoggedInGame = true;
                Extensions.DateTimeExtensions.UpdateServerTime(packet.serverTime);
                UIService.instance.ExecuteInstruction((object Obj) =>
                {
                    UIService.instance.HideAll();
                    UIService.instance.Show(new GameObject[] { UIService.instance.ChatUI, UIService.instance.BattleSelectorUI, UIService.instance.PlayerPanelUI, UIService.instance.BackgroundUI});//, ClientInit.uiManager.BattlesMainLobbyUI 
                    //ClientInit.uiManager.GarageUI.GetComponent<GarageUIHandler>().
                    var loginhandler = UIService.instance.LoginRegisterUI.GetComponent<LoginRegistrationUIHandler>();
                    if (loginhandler.LoginAutosave.isOn || loginhandler.RegisterAutosave.isOn)
                    {
                        File.WriteAllText(Application.streamingAssetsPath + "/loginconfig.json",
                    "{\"LoginData\":{\"login\":\"" + lastSendedAutentificationCredentials.Item1 + "\",\"password\":\"" + lastSendedAutentificationCredentials.Item2 + "\"}}");
                    }
                }, null);
                Socket.emit<GetUserViaUsername>(new GetUserViaUsername
                {
                    packetId = Guid.NewGuid
                ().GetHashCode(),
                    Username = Username
                });
                
            });

            Socket.on<CaptchaRequiredEvent>(packet => {
                CaptchaRequired(packet);
            });

            Socket.on<AvailableResult>((AvailableResult packet) => {
                //ULogger.Log(packet);
                //CaptchaRequired(packet);
            });

            Socket.on(async (GameDataEvent gameDataEvent) =>
            {
                Type eventType;
                //if (ManagerScope.eventManager.AcceptedOutsideEvents.Contains(gameDataEvent.typeId))
                {
                    if (ManagerScope.eventManager.EventSerializationCache.TryGetValue(gameDataEvent.typeId, out eventType))
                    {
                        ULogger.Log(eventType);
                        await Task.Run(() =>
                        {
                            using (var reader = new StringReader(gameDataEvent.jsonData))
                            {
                                var unserializedEvent = (ECSEvent)new JsonSerializer().Deserialize(reader, eventType);
                                //unserializedEvent.user = Socket;
                                ManagerScope.eventManager.OnEventAdd(unserializedEvent);
                            }

                        }).LogExceptionIfFaulted().ConfigureAwait(false);
                    }
                }

            });


            Socket.on(async (RawMovementEvent rawMovementEvent) =>
            {
                //EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager>(Ma)
                if(ManagerScope.entityManager.EntityStorage.TryGetValue(rawMovementEvent.PlayerEntityId, out var moveEntity))
                {
                    if(moveEntity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                    {
                        tankManager.ExecuteInstruction(() => tankManager.hullManager.chassisManager.SynchronizePosition(rawMovementEvent));
                    }
                }
            });
            Socket.on(async (RawDropTakingEvent rawDropTakingEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new BonusTakenEvent()
                    {
                        cachedRawEvent = rawDropTakingEvent,
                        EntityOwnerId = rawDropTakingEvent.PlayerEntityId,
                        DropId = rawDropTakingEvent.dropEntityId
                    });
                    }
                ).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawHitEvent rawHitEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new HitEvent()
                    {
                        cachedRawEvent = rawHitEvent,
                        EntityOwnerId = rawHitEvent.PlayerEntityId,
                        hitDistanceList = rawHitEvent.hitDistanceList,
                        hitList = rawHitEvent.hitList,
                        hitLocalDistanceList = rawHitEvent.hitLocalDistanceList
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawCreatureActuationEvent rawCreatureActuationEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new CreatureActuationEvent()
                    {
                        cachedRawEvent = rawCreatureActuationEvent,
                        CreatureInstanceId = rawCreatureActuationEvent.CreatureInstanceId,
                        BattleDBOwnerId = rawCreatureActuationEvent.BattleDBOwnerId,
                        TargetsId = rawCreatureActuationEvent.TargetsId
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawShotEvent rawShotEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new ShotEvent()
                    {
                        cachedRawEvent = rawShotEvent,
                        EntityOwnerId = rawShotEvent.PlayerEntityId,
                        hitDistanceList = rawShotEvent.hitDistanceList,
                        hitList = rawShotEvent.hitList,
                        MoveDirectionNormalized = rawShotEvent.MoveDirectionNormalized,
                        StartGlobalPosition = rawShotEvent.StartGlobalPosition,
                        StartGlobalRotation = rawShotEvent.StartGlobalRotation,
                        hitLocalDistanceList = rawShotEvent.hitLocalDistanceList
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawUpdateBattleCreaturesEvent rawUpdateBattleCreaturesEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new UpdateBattleCreaturesEvent()
                    {
                        cachedRawEvent = rawUpdateBattleCreaturesEvent,
                        BattleId = rawUpdateBattleCreaturesEvent.BattleId,
                        appendCreatures = rawUpdateBattleCreaturesEvent.appendCreatures,
                        removeCreatures = rawUpdateBattleCreaturesEvent.removeCreatures
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawStartShootingEvent rawStartShootingEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new StartShootingEvent()
                    {
                        cachedRawEvent = rawStartShootingEvent,
                        EntityOwnerId = rawStartShootingEvent.PlayerEntityId
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawEndShootingEvent rawStopShootingEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new EndShootingEvent()
                    {
                        cachedRawEvent = rawStopShootingEvent,
                        EntityOwnerId = rawStopShootingEvent.PlayerEntityId
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawSupplyUsingEvent rawSupplyUsingEvent) =>
            {
                await Task.Run(() => {
                    ManagerScope.eventManager.OnEventAdd(new SupplyUsedEvent()
                    {
                        cachedRawEvent = rawSupplyUsingEvent,
                        targetEntities = rawSupplyUsingEvent.targetEntities,
                        EntityOwnerId = rawSupplyUsingEvent.PlayerEntityId,
                        supplyPath = rawSupplyUsingEvent.supplyPath
                    });
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawHealthComponentDirectUpdateEvent rawHealthComponentDirectUpdateEvent) =>
            {
                await Task.Run(() => {
                    ECSEntity player = null;
                    if (ManagerScope.entityManager.EntityStorage.TryGetValue(rawHealthComponentDirectUpdateEvent.PlayerUpdateEntityId, out player))
                    {
                        var healthComponent = player.GetComponent<HealthComponent>();
                        if (healthComponent == null)
                        {
                            var newHealthComponent = new HealthComponent()
                            {
                                CurrentHealth = rawHealthComponentDirectUpdateEvent.CurrentHealth,
                                MaxHealth = rawHealthComponentDirectUpdateEvent.MaxHealth
                            }.AddComponentGroup(new ServerComponentGroup());
                            newHealthComponent.instanceId = rawHealthComponentDirectUpdateEvent.ComponentInstanceId;
                            player.AddComponent(newHealthComponent);
                        }
                        else
                        {
                            healthComponent.CurrentHealth = rawHealthComponentDirectUpdateEvent.CurrentHealth;
                            healthComponent.MaxHealth = rawHealthComponentDirectUpdateEvent.MaxHealth;
                            healthComponent.MarkAsChanged();
                        }
                    }
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            Socket.on(async (RawEnergyComponentDirectUpdateEvent rawEnergyComponentDirectUpdateEvent) =>
            {
                await Task.Run(() => {
                    ECSEntity player = null;
                    if (ManagerScope.entityManager.EntityStorage.TryGetValue(rawEnergyComponentDirectUpdateEvent.PlayerUpdateEntityId, out player))
                    {
                        var energyComponent = player.GetComponent<WeaponEnergyComponent>();
                        if (energyComponent == null)
                        {
                            var newEnergyComponent = new WeaponEnergyComponent()
                            {
                                Energy = rawEnergyComponentDirectUpdateEvent.Energy,
                                MaxEnergy = rawEnergyComponentDirectUpdateEvent.MaxEnergy
                            }.AddComponentGroup(new ServerComponentGroup());
                            newEnergyComponent.instanceId = rawEnergyComponentDirectUpdateEvent.ComponentInstanceId;
                            player.AddComponent(newEnergyComponent);
                        }
                        else
                        {
                            energyComponent.Energy = rawEnergyComponentDirectUpdateEvent.Energy;
                            energyComponent.MaxEnergy = rawEnergyComponentDirectUpdateEvent.MaxEnergy;
                            energyComponent.MarkAsChanged();
                        }
                    }
                }).LogExceptionIfFaulted().ConfigureAwait(false);

            });
            ServerAvailable = true;
            if (OnServerAvailableAction != null)
                OnServerAvailableAction();
        }

        public void AttemptLogin(string login, string password, string captchaResult, Action<LoginFailedEvent> loginFailedCallback)
        {
            Socket.on<LoginFailedEvent>(packet => {
                loginFailedCallback(packet);
            });
            lastSendedAutentificationCredentials = (login, password);
            Socket.emit<LoginEvent>(new LoginEvent
            {
                login = login,
                password = password,
                captchaResultHash = captchaResult
            });
        }

        public void AttemptRegister(string login, string password, string Email, string captchaResult, Action<RegisterUserFailed> loginFailedCallback)
        {
            Socket.on<RegisterUserFailed>(packet => {
                loginFailedCallback(packet);
            });
            lastSendedAutentificationCredentials = (login, password);
            Socket.emit<RegisterUserRequest>(new RegisterUserRequest
            {
                Username = login,
                Password = password,
                Email = Email,
                captchaResultHash = captchaResult
            });
        }

        public void AttemptRestoreConnection(string login, string password, string Email, string captchaResult, Action<RegisterUserFailed> loginFailedCallback)
        {
            Socket.on<RegisterUserFailed>(packet => {
                loginFailedCallback(packet);
            });
            Socket.emit<RegisterUserRequest>(new RegisterUserRequest
            {
                Username = login,
                Password = password,
                Email = Email,
                captchaResultHash = captchaResult
            });
        }

        public void UsernameAvailable(string login, int IdRequest, Action<AvailableResult, int> loginFailedCallback)
        {
            Socket.on<AvailableResult>(packet => {
                loginFailedCallback(packet, IdRequest);
            });
            Socket.emit<UsernameAvailableRequest>(new UsernameAvailableRequest
            {
                packetId = IdRequest,
                encryptedUsername = login,
            });
        }

        public override void InitializeProcess()
        {
            
        }

        public override void OnDestroyReaction()
        {
            
        }

        public override void PostInitializeProcess()
        {
            TaskEx.RunAsync(() =>
            {
                InitConnection();
            });
        }
    }
}
