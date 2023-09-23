using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1486736500959L)]
    public class NotificationGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public NotificationGroupComponent() { }
        public NotificationGroupComponent(ECSEntity Key) : base(Key)
        {
        }

        public NotificationGroupComponent(long Id) : base(Id)
        {
        }
    }
}
