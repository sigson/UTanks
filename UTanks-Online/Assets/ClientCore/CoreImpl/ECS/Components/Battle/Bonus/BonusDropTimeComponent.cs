using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(-7944772313373733709)]
    public class BonusDropTimeComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusDropTimeComponent() { }
        public BonusDropTimeComponent(double dropTimeAwait, bool Cycle, Action<ECSEntity> ondrop)
        {
            CycleMilliseconds = dropTimeAwait;
            this.OnDrop = ondrop;
            onEnd = (entity, timerComponent) => {
                var timerComp = timerComponent as BonusDropTimeComponent;
                if(timerComp.OnDrop != null)
                    timerComp.OnDrop(entity);
                if(Cycle)
                    timerComp.TimerStart(CycleMilliseconds, entity);
            };
        }
        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(CycleMilliseconds, entity);
        }
        public double CycleMilliseconds { get; set; }
        public Action<ECSEntity> OnDrop;
    }
}