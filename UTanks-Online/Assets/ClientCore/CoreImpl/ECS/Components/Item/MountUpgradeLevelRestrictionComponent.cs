using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Item
{
    [TypeUid(17924731840401L)]
    public class MountUpgradeLevelRestrictionComponent : AbstractRestrictionComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
    }
}
