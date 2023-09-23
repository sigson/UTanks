using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.Extensions;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.ECS.Systems.User
{
    public class UserUpdaterSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            //SystemEventHandler.Add(UserLogged.Id, new List<Func<ECSEvent, object>>() {
            //    (Event) => {
            //        AppendUserToECS((UserLogged)Event);
            //        return null;
            //    }
            //});
            //ComponentsOnChangeCallbacks.Add()
            this.Enabled = true;
            this.DelayRunMilliseconds = 40;
        }

        //public void AppendUserToECS(UserLogged userLogged)
        //{
        //    userLogged.userEntity.AddComponentSilent(new UserSocketComponent() { Socket = userLogged.networkSocket });
        //    EntitySerialization.SerializeEntity(userLogged.userEntity);
        //    ManagerScope.entityManager.OnAddNewEntity(userLogged.userEntity);
        //    //ManagerScope.eventManager.OnEventAdd(new CreateEntitiesEvent() { Entities = new List<ECSEntity>() { userLogged.userEntity } });
        //    ManagerScope.eventManager.OnEventAdd(new TransferEntitiesEvent() { Entities = new List<string>() { EntitySerialization.BuildSerializedEntityWithGDAP(userLogged.userEntity, userLogged.userEntity) }, EntityIdRecipient = userLogged.userEntity.instanceId, EntityOwnerId = userLogged.userEntity.instanceId });

        //    UpdateEventWatcher(userLogged);
        //}

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { UserOnlineComponent.Id}, IValues = { 0 } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { /*Keys = { UserLogged.Id }, Values = { 0 }*/ }.Upd();
        }

        public override void Run(long[] entities)
        {
            ParallelLoopResult result = Parallel.ForEach<long>(
                entities,
                (long entityId) => 
                {
                    var entity = ManagerScope.entityManager.EntityStorage[entityId];
                    lock(entity.contextSwitchLocker)
                    {
                        if (entity.HasComponent(BattleOwnerComponent.Id))
                            return;
                        EntitySerialization.SerializeEntity(entity, true);
                        
                        //if(entity.HasComponent(UserOnlineComponent.Id))
                        //    entity.RemoveComponent(UserOnlineComponent.Id);
                    }
                    var entitySerialized = EntitySerialization.BuildSerializedEntityWithGDAP(entity, entity);

                    //if (entitySerialized == "" && entity.entityComponents.Components.Count > 0)
                    //{
                    //    (entity.entityComponents.Components.ToArray()[0] as ECSComponent).MarkAsChanged();
                    //}
                    var userSocket = entity.GetComponent(UserSocketComponent.Id) as UserSocketComponent;
                    if (entitySerialized.Item1 != "")
                    {
                        UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
                        {
                            EntityIdRecipient = entityId,
                            Entities = new List<string>()
                        {
                            entitySerialized.Item1
                        }
                        };

                        Func<Task> asyncUpd = async () =>
                        {
                            await Task.Run(() => {
                                userSocket.Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
                            });
                        };
                        asyncUpd();
                    }
                    if(entitySerialized.Item2.Count > 0)
                    {
                        Func<Task> asyncUpd = async () =>
                        {
                            await Task.Run(() => {
                                foreach (var rawEvent in entitySerialized.Item2)
                                {
                                    userSocket.Socket.emit(rawEvent);
                                }
                            });
                        };
                        asyncUpd();
                    }
                    

                }
            );
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
