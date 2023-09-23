using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(8566120830355322079L)]
    public class BonusRegionGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusRegionGroupComponent() { }
        public BonusRegionGroupComponent(ECSEntity entity) : base(entity)
        {
        }

        public BonusRegionGroupComponent(long Key) : base(Key)
        {
        }
    }
}