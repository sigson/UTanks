using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.DailyBonus
{
    [TypeUid(636457555163806848L)]
    public class DailyBonusFirstCycleComponent : DailyBonusCycleComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
    }
}
