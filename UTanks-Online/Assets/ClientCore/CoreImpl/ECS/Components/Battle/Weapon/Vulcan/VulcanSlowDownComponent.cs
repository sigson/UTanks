using UTanksClient.Core;
//using UTanksClient.Core.Battles.BattleWeapons;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
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
