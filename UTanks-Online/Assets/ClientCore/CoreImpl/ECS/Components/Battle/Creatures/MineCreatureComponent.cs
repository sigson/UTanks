using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace UTanksClient.ECS.Components.Battle.Creatures
{
    [TypeUid(218120146282063460)]
    public class MineCreatureComponent : ICreatureComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WorldPoint minePoint;
        public MineState mineState;
        //public long explodedEntity;
        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);

            BattleManager.LoadedBattleClientAction(entity, (nul) =>
            {
                var battleManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(entity.instanceId);
                if (!battleManager.BattleCreatureDB.ContainsKey(this.instanceId))
                {
                    var newMine = MineCreature.Create(this, battleManager);
                    //newMine.CreatureOwnerId = entity.instanceId;
                    newMine.DBOwner = entity.instanceId;//ManagerScope.entityManager.EntityStorage[ClientInit.battleManager.NowBattleId].GetComponent<BattleCreatureStorageComponent>().InstanceId;
                    battleManager.BattleCreatureDB[this.instanceId] = newMine;
                    newMine.UpdateCreatureState(this, ComponentsDBComponent.ComponentState.Created);
                }
                else
                {
                    ULogger.Error("Creature already was created " + this.instanceId.ToString());
                }
            });

            //var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(entity);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            var battleManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(entity.instanceId);
            battleManager.ExecuteInstruction((object Obj) =>
            {
                try
                {
                    battleManager.BattleCreatureDB[this.instanceId].UpdateCreatureState(this, ComponentsDBComponent.ComponentState.Removed);
                    battleManager.BattleCreatureDB.Remove(this.instanceId);
                }
                catch
                {
                    ULogger.Error("error remove creature " + this.instanceId.ToString());
                }
                

            }, null);
        }
    }
    public enum MineState
    {
        Installed,
        Explosed,
        Dispelled
    }
}
