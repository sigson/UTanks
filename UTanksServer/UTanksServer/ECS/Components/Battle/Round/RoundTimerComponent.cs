using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;

namespace UTanksServer.ECS.Components.Battle.Round
{
    [TypeUid(231444874950937060)]
    public class RoundTimerComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public RoundTimerComponent() { }

        public RoundTimerComponent(int minutes)
        {
            this.timerAwait = minutes * 60;
            onEnd = (entity, timerComponent) => {
                this.ownerEntity.RemoveComponent(this.GetId());
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            if(this.timerAwait != 0)
                this.TimerStart(0f, null, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (this.timerAwait != 0)
            {
                this.TimerStop();
                ManagerScope.eventManager.OnEventAdd(new BattleEndEvent() { TeamWinnerInstanceId = 0, BattleEntity = this.ownerEntity.instanceId });
            }
        }
    }
}
