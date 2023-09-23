using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(-3961778961585441606L)]
    public class BonusRegionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusRegionComponent() { }
        public BonusRegionComponent(BonusType type)
        {
            Type = type;
        }

        public BonusType Type { get; set; }
    }
}