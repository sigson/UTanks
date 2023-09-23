using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.Location;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents.Shooting;
using UTanksServer.Network.NetworkEvents.FastGameEvents;

namespace UTanksServer.ECS.Components.Battle.BotComponent
{
    [TypeUid(223899603046641000)]
    public class AutoSmokyComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public AutoSmokyComponent() {}
        public AutoSmokyComponent(float period)
        {
            ShotPeriod = period;
            onEnd = (entity, timerComp) =>
            {
                try
                {
                    var battleEntity = entity.GetComponent<BattleOwnerComponent>().Battle;
                    var playerPreparedList = battleEntity.GetComponent<BattlePlayersComponent>().players.Keys.Where(x => x.instanceId != entity.instanceId && x.HasComponent(AutoSmokyComponent.Id)).ToList();
                    var battleRandomPlayer = playerPreparedList[new Random().Next(0, playerPreparedList.Count)];
                    var shotEvent = new ShotEvent()
                    {
                        EntityOwnerId = entity.instanceId,
                        hitDistanceList = new Dictionary<long, float>()
                    {
                        {battleRandomPlayer.instanceId, 0 }
                    },
                        hitList = new Dictionary<long, Types.Battle.Vector3S>()
                    {
                        {battleRandomPlayer.instanceId, battleRandomPlayer.GetComponent<WorldPositionComponent>().WorldPoint.Position }
                    },
                        hitLocalDistanceList = new Dictionary<long, float>()
                    {
                        {battleRandomPlayer.instanceId, 0 }
                    },
                        MoveDirectionNormalized = new Types.Battle.Vector3S(),
                        StartGlobalPosition = battleRandomPlayer.GetComponent<WorldPositionComponent>().WorldPoint.Position,
                        StartGlobalRotation = new Types.Battle.QuaternionS()
                    };
                    shotEvent.cachedRawEvent = new RawShotEvent()
                    {
                        hitDistanceList = shotEvent.hitDistanceList,
                        hitList = shotEvent.hitList,
                        hitLocalDistanceList = shotEvent.hitLocalDistanceList,
                        MoveDirectionNormalized = shotEvent.MoveDirectionNormalized,
                        PlayerEntityId = shotEvent.EntityOwnerId,
                        StartGlobalPosition = shotEvent.StartGlobalPosition,
                        StartGlobalRotation = shotEvent.StartGlobalRotation,
                        ClientTime = 0
                    };
                    ManagerScope.eventManager.OnEventAdd(shotEvent);
                }
                catch { }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(ShotPeriod, ownerEntity, true, true);
        }

        public float ShotPeriod;
    }
}
