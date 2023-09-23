using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.BattleRewards
{
    [TypeUid(1514202494334L)]
    public class LevelUpUnlockPersonalRewardComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public LevelUpUnlockPersonalRewardComponent() { }
        public LevelUpUnlockPersonalRewardComponent(List<ECSEntity> unlocked)
        {
            Unlocked = unlocked;
        }

        public List<ECSEntity> Unlocked { get; set; }
    }
}
