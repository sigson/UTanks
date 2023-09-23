using Newtonsoft.Json;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(6673681254298647708L)]
    public class TemperatureComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [System.NonSerialized]
        [JsonIgnore]
        public HullComponent hullComponent;

        public TemperatureComponent() 
        {
            SetupTemperatureMethod();
        }
        public TemperatureComponent(float temperature)
        {
            Temperature = temperature;
            SetupTemperatureMethod();
        }

        public void UpdateTemperature(ECSEntity damagerEntity, WeaponComponent weaponComponent, float appendingTemperature)
        {
            //DamageEffect = (DamageEffect + damage) / 2;
            var hullComponent = damagerEntity.GetComponent<HullComponent>();
            
        }

        private void SetupTemperatureMethod()
        {
            onEnd = (entity, timerComp) =>
            {
                var temperatureComponent = timerComp as TemperatureComponent;
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            //TimerStart(0.5f, entity, true, true);
        }

        public float Temperature { get; set; }

        [field: System.NonSerialized]
        [JsonIgnore]
        public float DamageEffect { get; set; }
    }
}