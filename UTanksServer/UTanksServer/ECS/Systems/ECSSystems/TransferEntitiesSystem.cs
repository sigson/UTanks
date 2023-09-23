using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.ECSEvents;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Systems.ECSSystems
{
    public class TransferEntitiesSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void UpdateEventWatcher(ECSEvent eCSEvent)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
