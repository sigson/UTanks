using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1503298026299)]
    public class LeagueGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public LeagueGroupComponent() { }
        public LeagueGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public LeagueGroupComponent(long Key) : base(Key)
        {
        }
    }
}
