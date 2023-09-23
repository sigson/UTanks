using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Tank
{
    [TypeUid(4349602566017552920L)]
    public class TankNewStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public TankNewStateComponent()
        {
            onEnd = (entity, component) =>
            {
                entity.RemoveComponent(TankNewStateComponent.Id);
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(2f, entity, true);
        }
    }
}