using UTanksServer.Core;
//using UTanksServer.Core.Battles;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(1399558738794728790)]
    public class UserReadyToBattleComponent : ECSComponent
    {
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		//public void OnAttached(Player player, Entity battleUser)
		//{
		//	if (player.BattlePlayer != null)
		//	    player.BattlePlayer.MatchPlayer.TankState = TankState.Spawn;
		//}
	}
}
