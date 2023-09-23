using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.ECSEvents;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Systems.User
{
    public class UserUpdaterSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
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
            //this.Enabled = true;
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
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
