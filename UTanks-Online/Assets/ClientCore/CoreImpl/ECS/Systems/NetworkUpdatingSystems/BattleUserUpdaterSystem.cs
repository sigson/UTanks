using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.ECSEvents;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Systems.Battles
{
    public class BattleUserUpdaterSystem : ECSSystem
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

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { BattlePlayersComponent.Id }, IValues = { 0 } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { /*Keys = { UserLogged.Id }, Values = { 0 }*/ }.Upd();
        }

        public override void Run(long[] entities)
        {
            //ParallelLoopResult result = Parallel.ForEach<long>(
            //    entities,
            //    (long entityId) =>
            //    {
            //        var entity = ManagerScope.entityManager.EntityStorage[entityId];
            //        var battleUsers = entity.GetComponent(BattlePlayersComponent.Id) as BattlePlayersComponent;
            //        foreach(var player in battleUsers.players.Keys)
            //        {
            //            foreach (var otherPlayer in battleUsers.players.Keys)
            //            {
            //                if(player.instanceId == otherPlayer.instanceId)
            //                {
            //                    continue;
            //                }
            //                var userData = EntitySerialization.BuildSerializedEntityWithGDAP(player, otherPlayer);
            //                if (userData == "")
            //                    return;
            //                UpdateEntitiesEvent updateEntitiesEvent = new UpdateEntitiesEvent()
            //                {
            //                    EntityIdRecipient = player.instanceId,
            //                    Entities = new List<string>()
            //                    {
            //                        userData
            //                    }
            //                };
            //                Func<Task> asyncUpd = async () =>
            //                {
            //                    await Task.Run(() => {
            //                        (player.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(updateEntitiesEvent.PackToNetworkPacket());
            //                    });
            //                };
            //                asyncUpd();
            //            }
            //        }
            //    }
            //);
            this.LastEndExecutionTimestamp = DateTime.Now.Ticks;
            this.InWork = false;
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
