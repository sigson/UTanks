using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.Extensions;

namespace Assets.ClientCore.CoreImpl.ECS.Systems.User
{
    public class UserInfoUpdateSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            ComponentsOnChangeCallbacks.Add(UserCrystalsComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    var nComponent = component as UserCrystalsComponent;
                    if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
                    {
                        UIService.instance.ExecuteInstruction((object Obj) =>
                        {
                            UIService.instance.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().ChangeCrystalCount(nComponent.UserCrystals);
                        }, null);
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(UserScoreComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    var nComponent = component as UserScoreComponent;
                    if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
                    {
                        UIService.instance.ExecuteInstruction((object Obj) =>
                        {
                            UIService.instance.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().ChangeScore(nComponent.GlobalScore);
                        }, null);
                    }
                }
            });
            ComponentsOnChangeCallbacks.Add(UserRankComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                    var nComponent = component as UserRankComponent;
                    if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
                    {
                        UIService.instance.ExecuteInstruction((object Obj) =>
                        {
                            UIService.instance.PlayerPanelUI.GetComponent<PlayerPanelUIHandler>().SetupRank(nComponent.Rank);
                            if(ClientInitService.instance.CheckEntityIsPlayer(entity))
                            {
                                entity.GetComponent<UserGarageDBComponent>().MarkAsChanged();
                            }
                        }, null);
                    }
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
