using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(4088029591333632383)]
    public class TankGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TankGroupComponent() { }
        public TankGroupComponent(ECSEntity entity) : base(entity)
        {
        }

        public TankGroupComponent(long Key) : base(Key)
        {
        }
    }
}