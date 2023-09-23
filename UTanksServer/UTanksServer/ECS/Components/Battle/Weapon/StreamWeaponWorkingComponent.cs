using UTanksServer.Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(971549724137995758L)]
	public class StreamWeaponWorkingComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public void OnAttached(Player player, Entity battleUser) =>
        //    player.BattlePlayer.MatchPlayer.TryDeactivateInvisibility();

        //public void OnRemove(Player player, Entity battleUser) =>
        //    player.BattlePlayer.MatchPlayer.StreamHitLengths.Clear();

        public int Time { get; set; }
	}
}
