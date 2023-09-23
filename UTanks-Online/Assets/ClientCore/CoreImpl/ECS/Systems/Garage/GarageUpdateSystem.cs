using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace UTanksClient.ECS.Systems.Garage
{
    public class GarageUpdateSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            ComponentsOnChangeCallbacks.Add(UserGarageDBComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    var nComponent = component as UserGarageDBComponent;
                    if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
                    {
                        UIService.instance.ExecuteInstruction((object Obj) =>
                        {
                            lock(nComponent.locker)
                            {
                                if (nComponent.garage != null && nComponent.selectedEquipment != null)
                                    UIService.instance.garageUIHandler.GarageUpdate(nComponent);
                            }
                            
                        }, null);
                    }
                }
            });
            //ComponentsOnChangeCallbacks.Add(UserCrystalsComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
            //    (entity, component) => {
            //        var nComponent = component as UserCrystalsComponent;
            //        if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
            //        {
            //            ClientInit.uiManager.ExecuteInstruction((object Obj) =>
            //            {
            //                ClientInit.uiManager.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().ChangeCrystalCount(nComponent.UserCrystals);
            //            }, null);
            //        }
            //    }
            //});
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
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { GarageBuyItemEvent.Id }, IValues = { 0 } }.Upd();
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
