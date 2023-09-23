using System.Collections.Generic;
using System.Text.Json.Serialization;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle;
using UTanksServer.ECS.Systems.Battles;

namespace UTanksServer.ECS.Components.Battle.Round
{
    [TypeUid(-5556650973238726161L)]
    public class RoundRestartingStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [System.NonSerialized]
        [JsonIgnore]
        public Dictionary<long, float> rewardStorage = new Dictionary<long, float>();

        public RoundRestartingStateComponent() { }
        public RoundRestartingStateComponent(float timeRestart)
        {
            TimeRestart = timeRestart;
            onEnd = (entity, timerComp) =>
            {
                entity.RemoveComponent(timerComp.GetId());
                ManagerScope.eventManager.OnEventAdd(new BattleStartEvent() { BattleId = entity.instanceId });
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(TimeRestart, entity, true);
        }

        public float TimeRestart;
    }
}