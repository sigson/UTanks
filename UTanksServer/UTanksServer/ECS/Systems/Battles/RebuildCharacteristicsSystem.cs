using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Systems.Garage
{
    public class RebuildCharacteristicsSystem : ECSSystem
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
            ComponentsOnChangeCallbacks.Add(CharacteristicTransformersComponent.Id, new List<Action<ECSEntity, ECSComponent>>() {
                (entity, transformerComp) => {
                    var transformerComponent = transformerComp as CharacteristicTransformersComponent;
                    Reflection.CopyProperties(transformerComponent.sourceDamageComponent, transformerComponent.damageComponent);
                    foreach(var characterRebuilderAction in transformerComponent.characteristicTransformers)
                    {
                        if(!transformerComponent.disabledTransformers.Keys.Contains(characterRebuilderAction.Key))
                        {
                            characterRebuilderAction.Value.TransformAction(entity, transformerComponent.damageComponent);
                        }
                    }
                }
            });
            //ComponentsOnChangeCallbacks.Add()
            this.DelayRunMilliseconds = 40;
            //this.Enabled = false;
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { Keys = {  }, Values = {  } }.Upd();
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
            throw new NotImplementedException();
        }
    }
}
