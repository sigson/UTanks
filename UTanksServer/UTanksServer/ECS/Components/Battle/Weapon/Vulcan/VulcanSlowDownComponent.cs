using UTanksServer.Core;
//using UTanksServer.Core.Battles.BattleWeapons;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
	[TypeUid(-6843896944033144903L)]
	public class VulcanSlowDownComponent : ECSComponent
	{
        //public void OnAttached(Player player, Entity weapon)
        //{
        //    if (!IsAfterShooting) return;
        //    ((Vulcan) player.BattlePlayer.MatchPlayer.Weapon).LastVulcanHeatTactTime = null;
        //    ((Vulcan) player.BattlePlayer.MatchPlayer.Weapon).VulcanShootingStartTime = null;
        //}
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public bool IsAfterShooting { get; set; }
		public int ClientTime { get; set; }
	}
}
