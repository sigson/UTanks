﻿using Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Battle.TankEvents;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(229182338035098880)]
    public class DamageEffect : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float SelfHealingPercentage;
        public WeaponComponent weaponComponent;

        public DamageEffect() { }

        public DamageEffect(ECSEntity entity, WeaponComponent damageComponent)//damageComponent
        {
            if (damageComponent is IsisDamageComponent)
            {
                SelfHealingPercentage = (damageComponent as IsisDamageComponent).selfHealingProperty;
            }
            else
            {
                SelfHealingPercentage = float.Parse(ConstantService.ConstantDB["garage\\weaponeffects\\selfhealingeffect"].Deserialized["grades"][damageComponent.ComponentGrade]["selfHealingProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture);
            }
            weaponComponent = damageComponent;
        }

        private bool CheckAccessEffect(ECSEntity ownerDamageEntity, ECSEntity otherDamageEntity)
        {
            var ownerDamageTeam = ownerDamageEntity.GetComponent<TeamComponent>();
            var otherDamageTeam = otherDamageEntity.GetComponent<TeamComponent>();
            var battleEntity = ownerDamageEntity.GetComponent<BattleOwnerComponent>().Battle;
            if (!battleEntity.HasComponent<DMComponent>())
            {
                if (ownerDamageTeam.instanceId == otherDamageTeam.instanceId && !battleEntity.GetComponent<BattleComponent>().enableTeamKilling)
                    return false;
            }
            return true;
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            var effectAggregator = entity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
            if (weaponComponent.GetId() == FlamethrowerDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(FlamethrowerDamageComponent.Id) as FlamethrowerDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;
                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, component.damagePerHitProperty);
                        }
                        else
                        {
                            health.CurrentHealth -= component.damagePerHitProperty;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }


                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == IsisDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(IsisDamageComponent.Id) as IsisDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;
                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, component.damagePerHitProperty);
                        }
                        else
                        {
                            health.CurrentHealth -= component.damagePerHitProperty;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }

                    
                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == SmokyDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(SmokyDamageComponent.Id) as SmokyDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    //component.locker.TryEnterReadLockAwaiter(1);
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;

                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        var damage = (float)Math.Round(component.minDamageProperty + ((component.maxDamageProperty - component.minDamageProperty) * randomSalt), 2);
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, damage);
                        }
                        else
                        {
                            health.CurrentHealth -= damage;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }
                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == RailgunDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(RailgunDamageComponent.Id) as RailgunDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    //component.locker.TryEnterReadLockAwaiter(1);
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;

                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        var damage = (float)Math.Round(component.minDamageProperty + ((component.maxDamageProperty - component.minDamageProperty) * randomSalt), 2);
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, damage);
                        }
                        else
                        {
                            health.CurrentHealth -= damage;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }
                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == ThunderDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(ThunderDamageComponent.Id) as ThunderDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    //component.locker.TryEnterReadLockAwaiter(1);
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;

                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        var damage = (float)Math.Round(component.minDamageProperty + ((component.maxDamageProperty - component.minDamageProperty) * randomSalt), 2);
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, damage);
                        }
                        else
                        {
                            health.CurrentHealth -= damage;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }
                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == TwinsDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(TwinsDamageComponent.Id) as TwinsDamageComponent;
                    var health = otherEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    var resistanceDB = otherEntity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
                    //component.locker.TryEnterReadLockAwaiter(1);
                    lock (health.locker)
                    {
                        if (otherEntity.HasComponent(TankDeadStateComponent.Id) || otherEntity.HasComponent(TankNewStateComponent.Id) || health.CurrentHealth <= 0f)
                            return;

                        Action<ECSComponent, ECSComponent, ECSComponent, float> resistanceMethod;
                        var resultDamage = health.CurrentHealth;
                        var damage = (float)Math.Round(component.minDamageProperty + ((component.maxDamageProperty - component.minDamageProperty) * randomSalt), 2);
                        if (resistanceDB.resistanceAggregator.TryGetValue(component.GetId(), out resistanceMethod))
                        {
                            resistanceMethod(component, this, health, damage);
                        }
                        else
                        {
                            health.CurrentHealth -= damage;
                        }
                        resultDamage -= health.CurrentHealth;
                        if (health.CurrentHealth <= 0f)
                        {
                            health.CurrentHealth = 0f;
                            ManagerScope.eventManager.OnEventAdd(new KillEvent() { WhoKilledId = ownerEntity.instanceId, WhoDeadId = otherEntity.instanceId, EntityOwnerId = ownerEntity.instanceId, BattleId = (otherEntity.GetComponent(BattleOwnerComponent.Id) as BattleOwnerComponent).BattleInstanceId });
                            //otherEntity.AddComponent();
                        }
                        health.MarkAsChanged();
                    }
                    //component.locker.ExitReadLock();
                    //health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == FreezeDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                });
            }
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            //var effectAggregator = entity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
            //if(!effectAggregator.effectsAggregator.TryRemove(this.InstanceId, out _))
            //{
            //    Logger.LogError("error effect remove");
            //}
        }
    }
}
