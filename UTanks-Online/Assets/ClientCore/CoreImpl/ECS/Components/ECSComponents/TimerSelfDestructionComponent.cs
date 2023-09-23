using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.ECSComponents
{
    [TypeUid(221868751152916540)]
    public class TimerSelfDestructionComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TimerSelfDestructionComponent() { }
        public TimerSelfDestructionComponent(float timeSelfDestruction, Func<ECSEntity, bool> SelfDestructionCondition, Action<ECSEntity> selfDestructAction = null)
        {
            timerAwait = timeSelfDestruction * 1000;
            onEnd = (entity, selfDestructComponent) =>
            {
                if(SelfDestructionCondition(entity))
                {
                    if(selfDestructAction == null)
                    {
                        ManagerScope.entityManager.OnRemoveEntity(entity);
                    }
                    else
                    {
                        selfDestructAction(entity);
                    }
                    TimerStop();
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(timerAwait, entity, false, true);
        }
    }
}
