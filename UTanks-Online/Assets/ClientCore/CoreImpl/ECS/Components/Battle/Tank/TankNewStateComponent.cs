using SecuredSpace.Battle.Tank;
using UTanksClient.Core.Logging;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.Components.Battle.Location;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(4349602566017552920L)]
    public class TankNewStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public TankNewStateComponent()
        {
            onEnd = (entity, component) =>
            {
                
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            //if (ManagerScope.entityManager.EntityStorage.Count == 0)
            //    return;
            //this.TimerStart(2f, entity, true);
            if(entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                try
                {
                    ClientInitService.instance.ExecuteInstruction(() =>
                    {
                        tankManager.SpawnTank(entity, entity.GetComponent<WorldPositionComponent>(WorldPositionComponent.Id).WorldPoint);
                        tankManager.EnableGhostTankState();
                    });
                }
                catch
                {
                    ULogger.Error("error spawn tank");
                }
            }
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            //entity.RemoveComponent(TankNewStateComponent.Id);
            if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                try
                {
                    tankManager.ExecuteInstruction(() => tankManager.DisableGhostTankState());
                }
                catch
                {
                    ULogger.Error("error disable ghost");
                }
            }
        }
    }
}