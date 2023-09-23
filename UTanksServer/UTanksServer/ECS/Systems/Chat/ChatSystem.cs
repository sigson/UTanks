using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Chat;
using UTanksServer.ECS.Systems.User;
using UTanksServer.ECS.Templates.User;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Systems.Chat
{
    public class ChatSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(ChatSendMessageEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    ChatMessageAccept((ChatSendMessageEvent)Event);
                    return null;
                }
            });
            //ComponentsOnChangeCallbacks.Add()
            //this.Enabled = true;
        }

        private void ChatMessageAccept(ChatSendMessageEvent chatSendMessageEvent)
        {
            if(chatSendMessageEvent.messageBody == "")
            {
                return;
            }
            var playerEntity = ManagerScope.entityManager.EntityStorage[chatSendMessageEvent.EntityOwnerId];
            if(chatSendMessageEvent.messageBody[0] == '/')
            {
                try
                {
                    var playerPrivilege = playerEntity.GetComponent<UserPrivilegeGroupComponent>(UserPrivilegeGroupComponent.Id);
                    if (chatSendMessageEvent.messageBody.Substring(1, "addscore".Length) == "addscore")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "testserver")
                        {
                            UserTemplate.ScoreUpdate(playerEntity, int.Parse(chatSendMessageEvent.messageBody.Substring(1 + "addscore".Length + 1)));

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "addcrystal".Length) == "addcrystal")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "testserver")
                        {
                            var playerCrystals = playerEntity.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id);
                            playerCrystals.UserCrystals += int.Parse(chatSendMessageEvent.messageBody.Substring(1 + "addcrystal".Length + 1));
                            playerCrystals.MarkAsChanged();
                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "ban".Length) == "ban")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "kick".Length) == "kick")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "chatban".Length) == "chatban")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "stopbattle".Length) == "stopbattle")//stop battle without fund distribution
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "endbattle".Length) == "endbattle")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {

                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "spawngold".Length) == "spawngold")
                    {
                        if (playerPrivilege.PrivilegeGroup == "admin" || playerPrivilege.PrivilegeGroup == "maintester" || playerPrivilege.PrivilegeGroup == "gamemoderator")
                        {
                            var battleOwner = playerEntity.GetComponent<BattleOwnerComponent>();
                            if (battleOwner != null)
                            {
                                var dropStorage = ManagerScope.entityManager.EntityStorage[battleOwner.Battle.GetComponent<BattleDropRegionsComponent>().dropRegions[Types.Battle.Bonus.BonusType.Gold][0]].GetComponent<BonusRegionDropStorageComponent>();
                                for (int i = 0; i < int.Parse(chatSendMessageEvent.messageBody.Substring(1 + "spawngold".Length + 1)); i++)
                                {
                                    dropStorage.DropStorage.Add(new BonusDropRecord(0, Types.Battle.Bonus.BonusType.Gold));
                                }
                            }
                        }
                    }
                    if (chatSendMessageEvent.messageBody.Substring(1, "vote".Length) == "vote" || chatSendMessageEvent.messageBody.Substring(1, "voteban".Length) == "voteban")
                    {

                    }
                }
                catch { }
            }
            else
            {
                var senderEntity = ManagerScope.entityManager.EntityStorage[chatSendMessageEvent.EntityOwnerId];
                chatSendMessageEvent.cachedGameDataEvent = null;
                chatSendMessageEvent.SenderNickname = senderEntity.GetComponent<UsernameComponent>().Username;
                chatSendMessageEvent.SenderRank = senderEntity.GetComponent<UserRankComponent>().Rank;
                if (chatSendMessageEvent.battleEntity != 0)
                {
                    var battleComponent = playerEntity.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id);
                    if (battleComponent != null)
                    {
                        var playerTeam = playerEntity.GetComponent<TeamComponent>(TeamComponent.Id);
                        var players = battleComponent.Battle.GetComponent<BattlePlayersComponent>(BattlePlayersComponent.Id);
                        foreach(var player in players.players)
                        {
                            if(chatSendMessageEvent.teamMessage)
                            {
                                if(player.Value == playerTeam)
                                {
                                    Func<Task> asyncUpd = async () =>
                                    {
                                        await Task.Run(() => {
                                            (player.Key.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(chatSendMessageEvent.PackToNetworkPacket());
                                        });
                                    };
                                    asyncUpd();
                                }
                            }
                            else
                            {
                                Func<Task> asyncUpd = async () =>
                                {
                                    await Task.Run(() => {
                                        (player.Key.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(chatSendMessageEvent.PackToNetworkPacket());
                                    });
                                };
                                asyncUpd();
                            }
                        }
                    }
                }
                else
                {
                    var playerList = ManagerScope.systemManager.SystemsInterestedEntityDatabase.Where(x => x.Key.GetType() == typeof(UserUpdaterSystem)).ToList()[0].Value.Keys.ToList();
                    foreach(var playerId in playerList)
                    {
                        var player = ManagerScope.entityManager.EntityStorage[playerId];
                        if(!player.HasComponent(BattleOwnerComponent.Id))
                        {
                            Func<Task> asyncUpd = async () =>
                            {
                                await Task.Run(() => {
                                    (player.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(chatSendMessageEvent.CachePackToNetworkPacket());
                                });
                            };
                            asyncUpd();
                        }
                    }
                }
            }
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { }, IValues = { } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { ChatSendMessageEvent.Id }, IValues = { 0 } }.Upd();
        }

        public override void Run(long[] entities)
        {

        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
