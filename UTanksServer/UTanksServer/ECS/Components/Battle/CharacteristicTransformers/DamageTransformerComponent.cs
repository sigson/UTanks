using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.Battle.Weapon.Effects;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.CharacteristicTransformers
{
    [TypeUid(216440710885484540)]
    public class DamageTransformerComponent : TimerComponent, ICharacteristicTransformerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [NonSerialized]
        static public ConfigObj configObj;
        [NonSerialized]
        static public float damageEffect;
        public double TransformerTimerAwait;
        public DamageTransformerComponent() { }
        
        private void InitializeTransformers()
        {
            TransformAction = (entity, component) =>
            {
                #region turrets
                if (entity.HasComponent<SmokyDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<SmokyDamageComponent>();
                    var baseComponent = entity.GetComponent<SmokyComponent>();
                    InfluenceWeaponStorage = new SmokyComponent()
                    {
                        maxDamageProperty = baseComponent.maxDamageProperty * damageEffect,
                        minDamageProperty = baseComponent.minDamageProperty * damageEffect,
                        criticalDamageProperty = baseComponent.criticalDamageProperty * damageEffect,
                    };
                    damageComponent.maxDamageProperty += baseComponent.maxDamageProperty * damageEffect;
                    damageComponent.minDamageProperty += baseComponent.minDamageProperty * damageEffect;
                    damageComponent.criticalDamageProperty += baseComponent.criticalDamageProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<RailgunDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<RailgunDamageComponent>();
                    var baseComponent = entity.GetComponent<RailgunComponent>();
                    InfluenceWeaponStorage = new RailgunComponent()
                    {
                        maxDamageProperty = baseComponent.maxDamageProperty * damageEffect,
                        minDamageProperty = baseComponent.minDamageProperty * damageEffect,
                    };
                    damageComponent.maxDamageProperty += baseComponent.maxDamageProperty * damageEffect;
                    damageComponent.minDamageProperty += baseComponent.minDamageProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<IsisDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<IsisDamageComponent>();
                    var baseComponent = entity.GetComponent<IsisComponent>();
                    InfluenceWeaponStorage = new IsisComponent()
                    {
                        damagePerSecondProperty = baseComponent.damagePerSecondProperty * damageEffect,
                        healingProperty = baseComponent.healingProperty * damageEffect,
                        selfHealingProperty = baseComponent.selfHealingProperty * damageEffect
                    };
                    damageComponent.damagePerSecondProperty += baseComponent.damagePerSecondProperty * damageEffect;
                    damageComponent.healingProperty += baseComponent.healingProperty * damageEffect;
                    damageComponent.selfHealingProperty += baseComponent.selfHealingProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<FlamethrowerDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<FlamethrowerDamageComponent>();
                    var baseComponent = entity.GetComponent<FlamethrowerComponent>();
                    InfluenceWeaponStorage = new FlamethrowerComponent()
                    {
                        damagePerSecondProperty = baseComponent.damagePerSecondProperty * damageEffect
                    };
                    damageComponent.damagePerSecondProperty += baseComponent.damagePerSecondProperty * damageEffect;
                    //damageComponent.healingProperty += damageComponent.healingProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<TwinsDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<TwinsDamageComponent>();
                    var baseComponent = entity.GetComponent<TwinsComponent>();
                    InfluenceWeaponStorage = new TwinsComponent()
                    {
                        maxDamageProperty = baseComponent.maxDamageProperty * damageEffect,
                        minDamageProperty = baseComponent.minDamageProperty * damageEffect
                    };
                    damageComponent.maxDamageProperty += baseComponent.maxDamageProperty * damageEffect;
                    damageComponent.minDamageProperty += baseComponent.minDamageProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                #endregion
            };

            UndoTransformAction = (entity, component) =>
            {
                #region turrets
                if (entity.HasComponent<SmokyDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<SmokyDamageComponent>();
                    var influence = InfluenceWeaponStorage as SmokyComponent;
                    damageComponent.maxDamageProperty -= influence.maxDamageProperty;
                    damageComponent.minDamageProperty -= influence.minDamageProperty;
                    damageComponent.criticalDamageProperty -= influence.criticalDamageProperty;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<RailgunDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<RailgunDamageComponent>();
                    var influence = InfluenceWeaponStorage as RailgunComponent;
                    damageComponent.maxDamageProperty -= influence.maxDamageProperty;
                    damageComponent.minDamageProperty -= influence.minDamageProperty;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<IsisDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<IsisDamageComponent>();
                    var influence = InfluenceWeaponStorage as IsisComponent;
                    damageComponent.damagePerSecondProperty -= influence.damagePerSecondProperty;
                    damageComponent.healingProperty -= influence.healingProperty;
                    damageComponent.selfHealingProperty -= influence.selfHealingProperty;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<FlamethrowerDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<FlamethrowerDamageComponent>();
                    var influence = InfluenceWeaponStorage as FlamethrowerComponent;
                    damageComponent.damagePerSecondProperty -= influence.damagePerSecondProperty;
                    //damageComponent.healingProperty -= damageComponent.healingProperty * damageEffect;
                    //damageComponent.MarkAsChanged();
                }
                if (entity.HasComponent<TwinsDamageComponent>())
                {
                    var damageComponent = entity.GetComponent<TwinsDamageComponent>();
                    var influence = InfluenceWeaponStorage as TwinsComponent;
                    damageComponent.maxDamageProperty -= influence.maxDamageProperty;
                    damageComponent.minDamageProperty -= influence.minDamageProperty;
                    //damageComponent.MarkAsChanged();
                }
                #endregion
            };
        }

        public DamageTransformerComponent(bool drop, float modificatorCoef = 1f)
        {
            if (configObj == null)
            {
                configObj = ConstantService.ConstantDB["battle\\modificator\\damage"];
                damageEffect = float.Parse(configObj.Deserialized["damageEffect"]["factor"].ToString(), CultureInfo.InvariantCulture);
            }

            InitializeTransformers();
            if (drop)
                TransformerTimerAwait = float.Parse(configObj.Deserialized["dropDuration"]["duration"].ToString(), CultureInfo.InvariantCulture);
            else
                TransformerTimerAwait = float.Parse(configObj.Deserialized["supplyDuration"]["duration"].ToString(), CultureInfo.InvariantCulture);
            TransformerTimerAwait *= modificatorCoef;
            onEnd = (entity, timerComponent) => {
                //UndoTransformAction(entity, timerComponent);
                entity.TryRemoveComponent(timerComponent.GetId());
            };
        }
        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            entity.GetComponent<CharacteristicTransformersComponent>().characteristicTransformers.TryAdd(this.instanceId, this);
            this.TimerStart(TransformerTimerAwait, entity, false, false);
            TransformAction(entity, this);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            this.TimerStop();
            entity.GetComponent<CharacteristicTransformersComponent>().characteristicTransformers.TryRemove(this.instanceId, out _);
            UndoTransformAction(entity, this);
        }
        [field: NonSerialized]
        [JsonIgnore]
        public Action<ECSEntity, ECSComponent> TransformAction { get; set; }
        [field: NonSerialized]
        [JsonIgnore]
        public Action<ECSEntity, ECSComponent> UndoTransformAction { get; set; }

        public bool stableTransformer { get; set; } = false;
        [field: NonSerialized]
        [JsonIgnore]
        public WeaponComponent InfluenceWeaponStorage { get; set; } = null;
        [field: NonSerialized]
        [JsonIgnore]
        public TankConstructionComponent InfluenceHullStorage { get; set; } = null;
    }
}
