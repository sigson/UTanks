using System;
using System.Threading.Tasks;
using UTanksClient.Core;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace UTanksClient.ECS.Components.Battle.Weapon
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

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            if(ClientInitService.instance.CheckEntityIsPlayer(entity) && !entity.HasComponent(StreamWeaponIdleComponent.Id))
            {
                TaskEx.RunAsync(() =>
                {
                    ClientNetworkService.instance.Socket.emit(new RawStartShootingEvent() { });
                });
            }
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (ClientInitService.instance.CheckEntityIsPlayer(entity) && !entity.HasComponent(StreamWeaponIdleComponent.Id))
            {
                TaskEx.RunAsync(() =>
                {
                    ClientNetworkService.instance.Socket.emit(new RawEndShootingEvent() { });
                });
            }
        }
    }
}
