using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.BattleRewards
{
    [TypeUid(636390744977660302L)]
    public class ChestBattleRewardComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ChestBattleRewardComponent() { }
        public ChestBattleRewardComponent(ECSEntity chest)
        {
            this.Chest = chest;
        }

        public ECSEntity Chest { get; set; }
    }
}
