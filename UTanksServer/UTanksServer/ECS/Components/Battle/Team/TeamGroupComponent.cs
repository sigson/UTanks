using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Team
{
    [TypeUid(6955808089218759626)]
    public class TeamGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TeamGroupComponent() { }
        public TeamGroupComponent(ECSEntity entity) : base(entity)
        {
        }

        public TeamGroupComponent(long Key) : base(Key)
        {
        }
    }
}