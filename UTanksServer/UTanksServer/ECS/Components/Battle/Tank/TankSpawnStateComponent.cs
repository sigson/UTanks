using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.ECS.Components.Battle.Tank
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
                var health = entity.GetComponent<HealthComponent>(HealthComponent.Id);
                health.CurrentHealth = health.MaxHealth;
                var energy = entity.GetComponent<WeaponEnergyComponent>(WeaponEnergyComponent.Id);
                energy.Energy = energy.MaxEnergy;
                health.MarkAsChanged();
                energy.MarkAsChanged();
                entity.RemoveComponent(TankSpawnStateComponent.Id);
                entity.AddComponent(new TankNewStateComponent().SetGlobalComponentGroup());
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(6f, entity, true);
        }
    }
}