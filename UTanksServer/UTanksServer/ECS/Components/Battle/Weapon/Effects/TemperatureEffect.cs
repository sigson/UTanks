using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.Weapon.Effects
{
    [TypeUid(193880908748133020)]
    public class TemperatureEffect : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float SelfHealingPercentage;
        public WeaponComponent weaponComponent;

        public TemperatureEffect() { }

        public TemperatureEffect(ECSEntity entity, WeaponComponent damageComponent)//damageComponent
        {
            if (damageComponent is IsisDamageComponent)
            {
                SelfHealingPercentage = (damageComponent as IsisDamageComponent).selfHealingProperty;
            }
            else
            {
                SelfHealingPercentage = float.Parse(ConstantService.ConstantDB["garage\\weaponeffects\\healingeffect"].Deserialized["grades"][damageComponent.ComponentGrade]["selfHealingProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture);
            }
            weaponComponent = damageComponent;
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            var effectAggregator = entity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
            if (weaponComponent.GetId() == FlamethrowerDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {

                });
            }
            else if (weaponComponent.GetId() == IsisDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    var component = ownerEntity.GetComponent(IsisDamageComponent.Id) as IsisDamageComponent;
                    
                });
            }
            else if (weaponComponent.GetId() == SmokyDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    var component = ownerEntity.GetComponent(SmokyDamageComponent.Id) as SmokyDamageComponent;
                    
                });
            }

        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            //var effectAggregator = entity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
            //if (!effectAggregator.effectsAggregator.TryRemove(this.InstanceId, out _))
            //{
            //    Logger.LogError("error effect remove");
            //}
        }
    }
}
