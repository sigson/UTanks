using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.CharacteristicTransformers
{
    [TypeUid(236021332241144200)]
    public class RepairTransformerComponent : TimerComponent, ICharacteristicTransformerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [NonSerialized]
        static public ConfigObj configObj;
        [NonSerialized]
        static public float tick;
        [NonSerialized]
        static public float healingFactor;
        [NonSerialized]
        public TimerEx healTimer;
        public float healValue;
        public double TransformerTimerAwait;
        [field: NonSerialized]
        [JsonIgnore]
        public WeaponComponent InfluenceWeaponStorage { get; set; } = null;
        [field: NonSerialized]
        [JsonIgnore]
        public TankConstructionComponent InfluenceHullStorage { get; set; } = null;
        public RepairTransformerComponent() { }

        private void InitializeTransformers()
        {
            TransformAction = (entity, component) =>
            {
                var healthComponent = entity.GetComponent<HealthComponent>();
                healTimer = new TimerEx();
                healTimer.Interval = tick;
                healTimer.Elapsed += async (sender, e) => await Task.Run(() => {
                    if (healthComponent.CurrentHealth + healValue <= healthComponent.MaxHealth)
                        healthComponent.CurrentHealth += healValue;
                    else
                    {
                        healthComponent.CurrentHealth = healthComponent.MaxHealth;
                        healTimer.Stop();
                        healthComponent.MarkAsChanged();
                        entity.TryRemoveComponent(RepairTransformerComponent.Id);                
                        return;
                    }       
                    healthComponent.MarkAsChanged();
                });
                healTimer.AutoReset = true;
                healTimer.Start();
                
                
                //hullComponent.MarkAsChanged();
            };

            UndoTransformAction = (entity, component) =>
            {
                healTimer.Stop();
                //hullComponent.MarkAsChanged();
            };
        }

        public RepairTransformerComponent(bool drop, float modificatorCoef = 1f)
        {
            if (configObj == null)
            {
                configObj = ConstantService.ConstantDB["battle\\modificator\\healing"];
                healingFactor = float.Parse(configObj.Deserialized["healing"]["hpPerMs"].ToString(), CultureInfo.InvariantCulture);
                tick = float.Parse(configObj.Deserialized["tick"]["period"].ToString(), CultureInfo.InvariantCulture);
            }

            InitializeTransformers();
            if (drop)
                TransformerTimerAwait = float.Parse(configObj.Deserialized["dropDuration"]["duration"].ToString(), CultureInfo.InvariantCulture);
            else
                TransformerTimerAwait = float.Parse(configObj.Deserialized["supplyDuration"]["duration"].ToString(), CultureInfo.InvariantCulture);
            healingFactor *= modificatorCoef;
            onEnd = (entity, timerComponent) =>
            {
                //UndoTransformAction(entity, timerComponent);
                entity.TryRemoveComponent(timerComponent.GetId());
            };
        }
        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            entity.GetComponent<CharacteristicTransformersComponent>().characteristicTransformers.TryAdd(this.instanceId, this);
            healValue = entity.GetComponent<HealthComponent>().MaxHealth * healingFactor;
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
    }
}