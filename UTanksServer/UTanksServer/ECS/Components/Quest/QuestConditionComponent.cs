using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1493901546731L)]
    public class QuestConditionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public Dictionary<QuestConditionType, ECSEntity> Condition { get; set; } = new Dictionary<QuestConditionType, ECSEntity>();
    }
}
