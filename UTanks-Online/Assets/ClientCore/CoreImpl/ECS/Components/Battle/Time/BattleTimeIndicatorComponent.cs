using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Time
{
    [TypeUid(1447751145383)]
    public class BattleTimeIndicatorComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleTimeIndicatorComponent() { }
        public BattleTimeIndicatorComponent(string timeText, float progress)
        {
            TimeText = timeText;
            Progress = progress;
        }

        public string TimeText { get; set; }
        
        public float Progress { get; set; }
    }
}