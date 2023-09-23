using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.User
{
    [TypeUid(636389758870600269)]
    public class GameplayChestScoreComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public GameplayChestScoreComponent() { }
        public GameplayChestScoreComponent(long current)
        {
            Current = current;
        }

        public long Current { get; set; }
        public long Limit { get; set; } = 1000;
    }
}
