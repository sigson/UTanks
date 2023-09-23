using Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Team;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.Weapon.Effects
{
    [TypeUid(223323484743283040)]
    public class SelfHealingEffect : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public float SelfHealingPercentage;
        public WeaponComponent weaponComponent;

        public SelfHealingEffect(){ }

        public SelfHealingEffect(ECSEntity entity, WeaponComponent damageComponent)//damageComponent
        {
            if(damageComponent is IsisDamageComponent)
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
            if(weaponComponent.GetId() == FlamethrowerDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(FlamethrowerDamageComponent.Id) as IsisDamageComponent;
                    var health = ownerEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    health.locker.EnterWriteLock();

                    if (health.CurrentHealth != health.MaxHealth)
                    {
                        component.locker.EnterReadLock();
                        var calcHeal = SelfHealingPercentage * component.damagePerHitProperty / 100;
                        if (health.CurrentHealth + calcHeal <= health.MaxHealth)
                        {
                            health.CurrentHealth += calcHeal;
                        }
                        else
                        {
                            health.CurrentHealth = health.MaxHealth;
                        }
                        health.MarkAsChanged();
                        component.locker.ExitReadLock();
                    }
                    health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == IsisDamageComponent.Id)
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(IsisDamageComponent.Id) as IsisDamageComponent;
                    var health = ownerEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    health.locker.EnterWriteLock();
                    
                    if(health.CurrentHealth != health.MaxHealth)
                    {
                        component.locker.EnterReadLock();
                        var calcHeal = SelfHealingPercentage * component.damagePerHitProperty / 100;
                        if (health.CurrentHealth + calcHeal <= health.MaxHealth)
                        {
                            health.CurrentHealth += calcHeal;
                        }
                        else
                        {
                            health.CurrentHealth = health.MaxHealth;
                        }
                        health.MarkAsChanged();
                        component.locker.ExitReadLock();
                    }
                    health.locker.ExitWriteLock();
                });
            }
            else if (weaponComponent.GetId() == SmokyDamageComponent.Id)//not full realized
            {
                effectAggregator.effectsAggregator.TryAdd(this.instanceId, (ownerEntity, otherEntity, randomSalt, hitEvent) =>
                {
                    if (!CheckAccessEffect(ownerEntity, otherEntity))
                        return;
                    var component = ownerEntity.GetComponent(SmokyDamageComponent.Id) as SmokyDamageComponent;
                    var health = ownerEntity.GetComponent(HealthComponent.Id) as HealthComponent;
                    //health.locker.EnterWriteLock();
                    lock (health.locker)
                    {
                        if (health.CurrentHealth != health.MaxHealth)
                        {
                            //component.locker.EnterReadLock();
                            var calcHeal = SelfHealingPercentage * component.minDamageProperty / 100;
                            if (health.CurrentHealth + calcHeal <= health.MaxHealth)
                            {
                                health.CurrentHealth += calcHeal;
                            }
                            else
                            {
                                health.CurrentHealth = health.MaxHealth;
                            }
                            health.MarkAsChanged();
                            //component.locker.ExitReadLock();
                        }
                    }
                    //health.locker.ExitWriteLock();
                });
            }

        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            //var effectAggregator = entity.GetComponent(EffectsAggregatorComponent.Id) as EffectsAggregatorComponent;
            //if (!effectAggregator.effectsAggregator.TryRemove(this.InstanceId, out _))
            //{
            //    Logger.LogError("error effect remove");
            //}
        }
    }
}
