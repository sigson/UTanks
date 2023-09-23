using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(6673681254298647708L)]
    public class TemperatureComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TemperatureComponent() { }
        public TemperatureComponent(float temperature)
        {
            Temperature = temperature;
        }

        public float Temperature { get; set; }
    }
}