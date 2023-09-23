using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Tank;
using SecuredSpace.UI.GameUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(-6549017400741137637)]
    public class BattleOwnerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        public ECSEntity Battle;
        public long BattleInstanceId;

        public BattleOwnerComponent() { }
        public BattleOwnerComponent(ECSEntity battle) 
        {
            this.Battle = battle;
            this.BattleInstanceId = battle.instanceId;
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TaskEx.RunAsync(() =>
            {
                if (this.ownerEntity == null || BattleInstanceId == 0)
                    return;
                int counter = 0;
                ECSEntity battleEntity = null;
                //var simpleInfo = battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo;
                while (counter < 300)
                {
                    if (!ManagerScope.entityManager.EntityStorage.ContainsKey(BattleInstanceId) || !ManagerScope.entityManager.EntityStorage[BattleInstanceId].HasComponent<BattleComponent>() || !ManagerScope.entityManager.EntityStorage[BattleInstanceId].HasComponent<BattleSimpleInfoComponent>())
                    {
                        Task.Delay(100).Wait();
                        counter++;
                        continue;
                    }
                    else
                    {
                        battleEntity = ManagerScope.entityManager.EntityStorage[BattleInstanceId];
                    }
                    if (!entity.HasComponent(UserBattleGarageDBComponent.Id))
                    {
                        Task.Delay(100).Wait();
                        counter++;
                        continue;
                    }
                    if (!battleEntity.HasComponent<BattleSimpleInfoComponent>())
                    {
                        Task.Delay(100).Wait();
                        counter++;
                        continue;
                    }
                    if (battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands.Values.Where(x => x.commandPlayers.ContainsKey(entity.instanceId)).Count() == 0)
                    {
                        Task.Delay(100).Wait();
                        counter++;
                        continue;
                    }
                    break;
                }
                if (counter > 298)
                {
                    ULogger.Error("A long time await for battle");
                    return;
                }

                if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                    EntityGroupManagersStorageService.instance.AddOrGetGroupManager<BattleManager, ECSEntity>(battleEntity);

                //battleManager.ExecuteInstruction(() => battleManager.LoadBattle(entity, battleEntity));

                BattleManager.LoadedBattleClientAction(battleEntity, (battleManagerObj) =>
                {
                    var battleManager = battleManagerObj as BattleManager;
                    battleManager.ExecuteFunction(() => TankManager.Create(entity, battleEntity));
                    if (!battleManager.TryGetValue(entity.instanceId, out _))
                    {
                        battleManager.Add(entity.instanceId, entity);
                    }
                    else
                    {
                        ULogger.Error("Tank already added");
                    }
                }, false, true);
            });
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (this.ownerEntity == null || BattleInstanceId == 0)
                return;


            BattleManager.LoadedBattleClientAction(entity, (battleManagerObj) =>
            {
                var battleManager = battleManagerObj as BattleManager;
                
                if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                {
                    battleManager.Remove(entity.instanceId);
                    entity.GetComponent<EntityManagersComponent>().Remove<TankManager>();
                    EntityGroupManagersStorageService.instance.RemoveGroupManager<BattleManager, ECSEntity>(battleManager.ConnectPoint);
                    //entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager);
                    //tankManager.RemoveTankFromBattle(tankManager.ConnectPoint as ECSEntity);

                }
                else
                {
                    if (battleManager.TryGetValue(entity.instanceId, out var leaver))
                    {
                        battleManager.Remove(leaver.instanceId);
                    }
                    else
                    {
                        ULogger.Log("error leave from battle, may be player exit first");
                    }
                }
                
            });

        }
    }
}