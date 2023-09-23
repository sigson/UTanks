using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Item
{
    [TypeUid(1799473840401L)]
    public class AbstractRestrictionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public int RestrictionValue { get; set; }
    }
}
