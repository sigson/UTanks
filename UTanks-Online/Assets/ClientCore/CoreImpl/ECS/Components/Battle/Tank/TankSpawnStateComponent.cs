using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Tank;
using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Types.Battle.AtomicType;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(-3257495205014980038)]
    public class TankSpawnStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WorldPoint spawnPosition;

        public TankSpawnStateComponent()
        {
            onEnd = (entity, component) => 
            {
                try { entity.RemoveComponent(TankDeadStateComponent.Id); } catch { }
                //entity.AddComponent(new TankNewStateComponent());
                if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                {
                    tankManager.ExecuteInstruction((object Obj) =>
                    {
                        tankManager.EnableGhostTankState();
                    }, null);
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            //this.TimerStart(6f, entity, true);
            if (this.ownerEntity == null)
                return;

            BattleManager.LoadedBattleClientAction(entity, (battleManagerObj) =>
            {
                var battleManager = battleManagerObj as BattleManager;
                battleManager.FollowCameraCurveMoveToPosition(spawnPosition);
            });
        }
    }
}