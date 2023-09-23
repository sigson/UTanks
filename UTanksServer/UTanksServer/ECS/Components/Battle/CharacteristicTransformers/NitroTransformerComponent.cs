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
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.CharacteristicTransformers
{
    [TypeUid(206099871123258300)]
    public class NitroTransformerComponent : TimerComponent, ICharacteristicTransformerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [NonSerialized]
        static public ConfigObj configObj;
        [NonSerialized]
        static public float speedCoeff;
        public double TransformerTimerAwait;
        [field: NonSerialized]
        [JsonIgnore]
        public WeaponComponent InfluenceWeaponStorage { get; set; } = null;
        [field: NonSerialized]
        [JsonIgnore]
        public TankConstructionComponent InfluenceHullStorage { get; set; } = null;
        public NitroTransformerComponent() { }

        private void InitializeTransformers()
        {
            TransformAction = (entity, component) =>
            {
                var hullComponent = entity.GetComponent<HullComponent>();
                hullComponent.speed += hullComponent.speed * speedCoeff;
                hullComponent.turnSpeed += hullComponent.turnSpeed * speedCoeff;
                hullComponent.acceleration += hullComponent.acceleration * speedCoeff;
                hullComponent.turnAcceleration += hullComponent.turnAcceleration * speedCoeff;
                hullComponent.reverseAcceleration += hullComponent.reverseAcceleration * speedCoeff;
                hullComponent.reverseTurnAcceleration += hullComponent.reverseTurnAcceleration * speedCoeff;
                hullComponent.MarkAsChanged();
            };

            UndoTransformAction = (entity, component) =>
            {
                var hullComponent = entity.GetComponent<HullComponent>();
                hullComponent.speed -= hullComponent.speed * speedCoeff;
                hullComponent.turnSpeed -= hullComponent.turnSpeed * speedCoeff;
                hullComponent.acceleration -= hullComponent.acceleration * speedCoeff;
                hullComponent.turnAcceleration -= hullComponent.turnAcceleration * speedCoeff;
                hullComponent.reverseAcceleration -= hullComponent.reverseAcceleration * speedCoeff;
                hullComponent.reverseTurnAcceleration -= hullComponent.reverseTurnAcceleration * speedCoeff;
                hullComponent.MarkAsChanged();
            };
        }

        public NitroTransformerComponent(bool drop, float modificatorCoef = 1f)
        {
            if(configObj == null)
            {
                configObj = ConstantService.ConstantDB["battle\\modificator\\turbospeed"];
                speedCoeff = float.Parse(configObj.Deserialized["turboSpeedEffect"]["speedCoeff"].ToString(), CultureInfo.InvariantCulture);
            }

            InitializeTransformers();
            if(drop)
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
    }
}
