using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle.Tank;
using UTanksClient.ECS.Components.Garage;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ComponentsGroup.Garage;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.ECS.Templates;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace UTanksClient.ECS.Systems.Garage
{
    class ChangeEquipmentSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(WeaponChangeEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    return null;
                }
            });
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
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { WeaponChangeEvent.Id }, IValues = { 0 } }.Upd();
        }

        public override void Run(long[] entities)
        {
            
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
