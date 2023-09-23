using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Bonus
{
    [TypeUid(185187698180891700)]
    public class BonusDespawningComponent_Deprecated : TimerComponent //deprecated, bonus now is creature
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusDespawningComponent_Deprecated() { }
        public BonusDespawningComponent_Deprecated(float despawnSecTime)
        {
            timerAwait = despawnSecTime * 1000;
            onEnd = (entity, selfDestructComponent) =>
            {
                var bonusState = entity.TryGetComponent<BonusStateComponent_Deprecated>();
                if(bonusState != null && (bonusState.bonusState != Types.Battle.Bonus.BonusState.Taken || bonusState.bonusState != Types.Battle.Bonus.BonusState.Taking))
                {
                    bonusState.bonusState = Types.Battle.Bonus.BonusState.Despawned;
                    bonusState.MarkAsChanged();
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(timerAwait, entity, false, false);
        }
    }
}
