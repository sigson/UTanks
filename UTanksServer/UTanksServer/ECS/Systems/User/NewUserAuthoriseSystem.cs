using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.ECS.Events.User;
using UTanksServer.ECS.Systems.Battles;
using UTanksServer.ECS.Templates;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Systems.User
{
    public class SystemNewUserAuthorise : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(UserLogged.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    AppendUserToECS((UserLogged)Event);
                    return null;
                }
            });
        }

        public void AppendUserToECS(UserLogged userLogged)
        {
            if(!userLogged.userRelogin)
            {
                userLogged.userEntity.AddComponentSilent(new UserSocketComponent() { Socket = userLogged.networkSocket });
                //userLogged.networkSocket.PlayerEntityId = userLogged.userEntity.instanceId;
                //EntitySerialization.SerializeEntity(userLogged.userEntity);
                ManagerScope.entityManager.OnAddNewEntity(userLogged.userEntity);
                userLogged.actionAfterLoggin(userLogged.userEntity);
                //ManagerScope.eventManager.OnEventAdd(new CreateEntitiesEvent() { Entities = new List<ECSEntity>() { userLogged.userEntity } });
                //var newPlayerEntity = ManagerScope.entityManager.EntityStorage[userLogged.instanceId];
            }
            else
            {
                userLogged.userEntity.GetComponent<UserSocketComponent>().Socket = userLogged.networkSocket;
                if(userLogged.userEntity.HasComponent(BattleOwnerComponent.Id))
                {
                    var battle = ManagerScope.entityManager.EntityStorage[userLogged.userEntity.GetComponent<BattleOwnerComponent>().BattleInstanceId];
                    var players = battle.GetComponent<BattlePlayersComponent>().players.Keys.ToList();
                    var playerData = new List<string>();
                    playerData.Add(EntitySerialization.BuildFullSerializedEntityWithGDAP(userLogged.userEntity, userLogged.userEntity));
                    foreach (var player in players)
                    {
                        if (player.instanceId == userLogged.userEntity.instanceId)
                        {
                            continue;
                        }
                        var otherData = EntitySerialization.BuildFullSerializedEntityWithGDAP(userLogged.userEntity, player);
                        if (otherData == "")
                            continue;
                        playerData.Add(otherData);
                    }
                    UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                    {
                        EntityIdRecipient = userLogged.userEntity.instanceId,
                        EntityOwnerId = userLogged.userEntity.instanceId,
                        Entities = playerData
                    };
                    userLogged.networkSocket.emit(updateEntitiesEvent.PackToNetworkPacket());
                    userLogged.userEntity.AddComponent(new UserOnlineComponent().SetGlobalComponentGroup());
                    //Func<Task> asyncUpd = async () =>
                    //{
                    //    await Task.Run(() => {

                    //    });
                    //};
                    //asyncUpd();
                }
                else
                {
                    userLogged.networkSocket.emit(new UpdateEntitiesEvent() { Entities = new List<string>() { EntitySerialization.BuildFullSerializedEntityWithGDAP(userLogged.userEntity, userLogged.userEntity) }, EntityIdRecipient = userLogged.userEntity.instanceId, EntityOwnerId = userLogged.userEntity.instanceId }.PackToNetworkPacket());
                    userLogged.userEntity.AddComponent(new UserOnlineComponent().SetGlobalComponentGroup());
                }
            }
            var battlesList = ManagerScope.systemManager.SystemsInterestedEntityDatabase.Where(x => x.Key.GetType() == typeof(BattleUpdaterSystem)).ToList()[0].Value.Keys.ToList();
            var entities = new List<string>();
            foreach(var battleEntityId in battlesList)
            {
                var battleEntity = ManagerScope.entityManager.EntityStorage[battleEntityId];
                entities.Add(EntitySerialization.BuildFullSerializedEntityWithGDAP(battleEntity, battleEntity));
            }
            //entities.Add(EntitySerialization.BuildFullSerializedEntityWithGDAP(userLogged.userEntity, userLogged.userEntity));
            userLogged.networkSocket.emit(new UpdateEntitiesEvent() { Entities = entities, EntityIdRecipient = userLogged.userEntity.instanceId, EntityOwnerId = userLogged.userEntity.instanceId }.PackToNetworkPacket());

            UpdateEventWatcher(userLogged);
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
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { UserLogged.Id }, IValues = { 0 } }.Upd();
        }

        public override void Run(long[] entities)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
