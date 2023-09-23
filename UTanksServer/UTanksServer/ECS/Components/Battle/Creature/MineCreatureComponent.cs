using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.ECS.Events.Battle.TankEvents.Shooting;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.Creature
{
    [TypeUid(218120146282063460)]
    public class MineCreatureComponent : ICreatureComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WorldPoint minePoint;
        public MineState mineState;
        [NonSerialized]
        public static Dictionary<int, float> MaxHealthForGrade = new Dictionary<int, float>();
        [NonSerialized]
        public static float DamagePercent = -1f;
        //public long explodedEntity;

        public override void OnCreatureActuation(List<ECSEntity> actuationTargets, BattleCreatureStorageComponent creatureStorage)
        {
            if(MaxHealthForGrade.Count == 0)
            {
                var mineConfig = ConstantService.GetByConfigPath(@"battle\creature\mine");
                DamagePercent = float.Parse(mineConfig.Deserialized["damagePercenageForGrade"]["initialValue"].ToString(), CultureInfo.InvariantCulture);
                var mammothConfig = ConstantService.GetByConfigPath(@"garage\hull\mammoth");
                mammothConfig.Deserialized["grades"].ForEach(grade => 
                {
                    MaxHealthForGrade[int.Parse(grade["grade"].ToString(), CultureInfo.InvariantCulture)] = float.Parse(grade["health"]["initialValue"].ToString(), CultureInfo.InvariantCulture);
                });
            }
            base.OnCreatureActuation(actuationTargets, creatureStorage);
            mineState = MineState.Explosed;
            foreach(var actuationTarget in actuationTargets)
            {
                if (actuationTarget != null && (!actuationTarget.HasComponent(TankDeadStateComponent.Id) && !actuationTarget.HasComponent(TankNewStateComponent.Id) && !actuationTarget.HasComponent(TankSpawnStateComponent.Id)))
                {
                    var randomSalt = (float)new Random().NextDouble();
                    var actuationTargetHealth = actuationTarget.GetComponent(HealthComponent.Id) as HealthComponent;
                    var oldHealth = actuationTargetHealth.CurrentHealth;
                    var resistanceDB = actuationTarget.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;

                    var damage = MaxHealthForGrade[actuationTarget.GetComponent<BaseHullComponent>().ComponentGrade] * DamagePercent;

                    lock (actuationTargetHealth.locker)
                    {
                        if (actuationTarget.HasComponent(TankDeadStateComponent.Id) || actuationTarget.HasComponent(TankNewStateComponent.Id) || actuationTargetHealth.CurrentHealth <= 0f)
                            return;
                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = actuationTargetHealth.CurrentHealth;
                        if (resistanceDB.resistanceAggregator.TryGetValue(this.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(this, this, actuationTargetHealth, damage);
                        }
                        else
                        {
                            actuationTargetHealth.CurrentHealth -= damage;
                        }
                        resultDamage -= actuationTargetHealth.CurrentHealth;
                        if (actuationTargetHealth.CurrentHealth <= 0f)
                        {
                            actuationTargetHealth.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = this.ownerEntity.instanceId, WhoDeadId = actuationTarget.instanceId, EntityOwnerId = this.ownerEntity.instanceId, BattleId = creatureStorage.ownerEntity.instanceId });
                            //otherEntity.AddComponent();
                        }
                        actuationTargetHealth.MarkAsChanged();
                    }

                    var newHealth = oldHealth - actuationTargetHealth.CurrentHealth;
                    (this.ownerEntity.GetComponent(UserSocketComponent.Id) as UserSocketComponent).Socket.emit(new ShotHPResultEvent() { Damage = newHealth, StruckEntityId = actuationTarget.instanceId }.PackToNetworkPacket());
                }
            }
            creatureStorage.RemoveComponent(this.instanceId, this.ownerEntity);
            creatureStorage.MarkAsChanged();
        }
    }
    public enum MineState
    {
        Installed,
        Explosed,
        Dispelled
    }
}
