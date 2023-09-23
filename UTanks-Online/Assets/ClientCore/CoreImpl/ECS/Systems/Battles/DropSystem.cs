using Assets.ClientCore.CoreImpl.ECS.Components.Battle.Bonus;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Drop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.BonusEvents;
using UTanksClient.Extensions;
using UTanksServer.ECS.Components.Battle;

namespace UTanksClient.ECS.Systems.Battles
{
    public class DropSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(BonusTakenEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    ClientInitService.instance.ExecuteInstruction((object Obj) =>
                    {
                        //ECSEntity battleEntity = null;
                        //Lambda.TryExecute(()=> {
                        //    battleEntity = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>((Event as BonusTakenEvent).EntityOwnerId).ConnectPoint as ECSEntity;
                        //});
                        //if(battleEntity != null)
                        //{
                        //    dropStorage battleEntity.GetComponent<BattleDropStorageComponent>();
                        //    var bonusState = dropEntity.GetComponent<BonusStateComponent>();
                        //    bonusState.bonusState = Types.Battle.Bonus.BonusState.Taken;
                        //    bonusState.MarkAsChanged();
                        //}
                        //else
                        //{
                        //    ULogger.Error("error bonus taken - battle not finded")
                        //}
                    }, null);
                    return null;
                }
            });
            ComponentsOnChangeCallbacks.Add(BonusComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, component) => {
                     ClientInitService.instance.ExecuteInstruction((object Obj) =>
                    {
                        if(!component.componentManagers.ContainsKey(typeof(DropManager)))
                            component.componentManagers.Add(typeof(DropManager), new DropManager());
                        DropManager.UpdateDrop(component as BonusComponent);
                    }, null);
                }
            });
            //this.Enabled = true;
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
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { BonusTakenEvent.Id }, IValues = { 0 } }.Upd();
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
