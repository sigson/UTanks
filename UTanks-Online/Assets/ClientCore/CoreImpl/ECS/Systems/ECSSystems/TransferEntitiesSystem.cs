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

namespace UTanksClient.ECS.Systems.ECSSystems
{
    public class TransferEntitiesSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(TransferEntitiesEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    TransferEntity(Event as TransferEntitiesEvent);
                    return null;
                }
            });
        }

        public static void TransferEntity(TransferEntitiesEvent transferEntitiesEvent)
        {
            var socket = (UserSocketComponent)ManagerScope.entityManager.EntityStorage[transferEntitiesEvent.EntityIdRecipient].GetComponent(UserSocketComponent.Id);
            ReceiveEntitiesEvent receiveEntitiesEvent = new ReceiveEntitiesEvent() { Entities = transferEntitiesEvent.Entities };
            socket.Socket.emit<GameDataEvent>(receiveEntitiesEvent.PackToNetworkPacket());
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { }, IValues = { } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() {
                IKeys = {
                    TransferEntitiesEvent.Id
                },
                IValues = { 0} }.Upd();
        }

        public override void Run(long[] entities)
        {
            
        }

        public override void UpdateEventWatcher(ECSEvent eCSEvent)
        {
            
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
