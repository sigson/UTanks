using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1506688142952)]
    public class QuestReadyComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
    }
}
