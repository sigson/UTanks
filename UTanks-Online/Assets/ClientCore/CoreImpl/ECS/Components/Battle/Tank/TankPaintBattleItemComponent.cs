using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(1437380560892)]
    public class TankPaintBattleItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
    }
}