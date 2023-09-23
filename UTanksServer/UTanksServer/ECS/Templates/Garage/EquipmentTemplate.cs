using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Chassis;
using UTanksServer.ECS.Components.Battle.Creature;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.Battle.Weapon.Effects;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ComponentsGroup.Garage;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Garage;
using UTanksServer.Services;

namespace UTanksServer.ECS.Templates
{
    [TypeUid(225826000761710720)]
    public class EquipmentTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 225826000761710720;

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }

        public void SetupEntity(ECSEntity entity)
        {
            var playerGarage = entity.GetComponent<UserBattleGarageDBComponent>(UserBattleGarageDBComponent.Id);
            UpdateEntity(entity, ConstantService.ConstantDB[playerGarage.battleEquipment.Hulls[0].PathName], new WeaponChangeEvent() { Grade = playerGarage.battleEquipment.Hulls[0].Grade });
            UpdateEntity(entity, ConstantService.ConstantDB[playerGarage.battleEquipment.Turrets[0].PathName], new WeaponChangeEvent() { Grade = playerGarage.battleEquipment.Turrets[0].Grade });
            UpdateEntity(entity, ConstantService.ConstantDB[playerGarage.battleEquipment.Colormaps[0].PathName], new WeaponChangeEvent() { Grade = 0});
            //UpdateEntity(entity, ConstantService.ConstantDB[playerGarage.selectedEquipment.TurretModules[0].PathName], new WeaponChangeEvent() { Grade = 0});
            entity.entityComponents.RegisterAllComponents();
        }

        public void UpdateEntity(ECSEntity entity, ConfigObj configObj, WeaponChangeEvent weaponChangeEvent)
        {
            //Deserialized["grades"][0]["priceItem"]["price"].ToString();
            switch (configObj.HeadLibName)
            {
                case "hull":
                    #region hull
                    entity.RemoveComponentsWithGroup(HullGroupComponent.Id);
                    HullComponentsUpdate(entity, configObj, weaponChangeEvent.Grade);
                    #endregion
                    break;
                case "weapon":
                    entity.RemoveComponentsWithGroup(TurretGroupComponent.Id);
                    TurretComponentsUpdate(entity, configObj, weaponChangeEvent.Grade);
                    break;
                case "turretmodules" or "hullmodules":
                    entity.RemoveComponentsWithGroup(TurretModuleGroupComponent.Id);
                    TurretModulesComponentsUpdate(entity, configObj);
                    break;
                case "colormap":
                    entity.RemoveComponentsWithGroup(ColormapGroupComponent.Id);
                    ColormapComponentsUpdate(entity, configObj);
                    break;
            }
        }

        private void HullComponentsUpdate(ECSEntity entity, ConfigObj configObj, int Grade)
        {
            var hullGroupComponent = new HullGroupComponent();
            var serialObject = configObj.Deserialized["grades"][Grade];

            var baseHullComponent = new BaseHullComponent
            { 
                acceleration = float.Parse(serialObject["acceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                damping = float.Parse(serialObject["damping"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                health = float.Parse(serialObject["health"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                hullType = serialObject["hullType"]["hullType"].ToString(),
                reverseAcceleration = float.Parse(serialObject["reverseAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                reverseTurnAcceleration = float.Parse(serialObject["reverseTurnAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                sideAcceleration = float.Parse(serialObject["sideAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                speed = float.Parse(serialObject["speed"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                turnAcceleration = float.Parse(serialObject["turnAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                turnSpeed = float.Parse(serialObject["turnSpeed"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                turretTurnAccelerationProperty = float.Parse(serialObject["turretTurnAccelerationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                turretTurnSpeedProperty = float.Parse(serialObject["turretTurnSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                weight = float.Parse(serialObject["weight"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                unloadTemperatureProperty = float.Parse(serialObject["unloadTemperatureProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                ComponentGrade = Grade,
                ConfigPath = new List<string> { serialObject.Path}
            }.AddComponentGroup(hullGroupComponent);

            var hullComponent = new HullComponent().UpdateComponent(baseHullComponent as TankConstructionComponent).AddComponentGroup(hullGroupComponent);

            var healthConfigComponent = new HealthConfigComponent(float.Parse(serialObject["health"]["initialValue"].ToString(), CultureInfo.InvariantCulture));
            healthConfigComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;

            var healthComponent = new HealthComponent(healthConfigComponent.BaseHealth);
            healthComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;


            var dampingComponent = new DampingComponent(float.Parse(serialObject["damping"]["initialValue"].ToString(), CultureInfo.InvariantCulture));
            dampingComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;

            var speedComponent = new SpeedComponent(float.Parse(serialObject["speed"]["initialValue"].ToString(), CultureInfo.InvariantCulture), float.Parse(serialObject["turnSpeed"]["initialValue"].ToString(), CultureInfo.InvariantCulture), float.Parse(serialObject["acceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture));
            speedComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;

            var configSpeedComponent = new SpeedConfigComponent(float.Parse(serialObject["turnAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture), float.Parse(serialObject["sideAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture), float.Parse(serialObject["reverseAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture), float.Parse(serialObject["reverseTurnAcceleration"]["initialValue"].ToString(), CultureInfo.InvariantCulture));
            configSpeedComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;

            var weightComponent = new WeightComponent(float.Parse(serialObject["weight"]["initialValue"].ToString(), CultureInfo.InvariantCulture));
            weightComponent.ComponentGroups[HullGroupComponent.Id] = hullGroupComponent;

            entity.AddComponentSilent(baseHullComponent);
            entity.AddComponentSilent(hullComponent);
            entity.AddComponentSilent(healthConfigComponent);
            entity.AddComponentSilent(healthComponent);
            entity.AddComponentSilent(dampingComponent);
            entity.AddComponentSilent(speedComponent);
            entity.AddComponentSilent(configSpeedComponent);
            entity.AddComponentSilent(weightComponent);
            entity.AddComponentSilent(new TemperatureComponent(0f).SetGlobalComponentGroup().AddComponentGroup(hullGroupComponent));
        }

        private void TurretComponentsUpdate(ECSEntity entity, ConfigObj configObj, int Grade)
        {
            var turretGroupComponent = new TurretGroupComponent();
            var serialObject = configObj.Deserialized["grades"][Grade];

            switch(configObj.LibName)
            {
                case "flamethrower":
                    if (true)
                    {
                        var weaponComponent = new FlamethrowerComponent
                        {
                            damagePerSecondProperty = float.Parse(serialObject["damagePerSecondProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            deltaTemperaturePerSecondProperty = float.Parse(serialObject["deltaTemperaturePerSecondProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            energyChargeSpeedProperty = float.Parse(serialObject["energyChargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            energyRechargeSpeedProperty = float.Parse(serialObject["energyRechargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            heatDamageProperty = float.Parse(serialObject["heatDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageDistanceProperty = float.Parse(serialObject["maxDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageDistanceProperty = float.Parse(serialObject["minDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamagePercentProperty = float.Parse(serialObject["minDamagePercentProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            temperatureLimitProperty = float.Parse(serialObject["temperatureLimitProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade
                        };
                        weaponComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;
                        var DamageComponent = new FlamethrowerDamageComponent().UpdateComponent(weaponComponent) as FlamethrowerDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = weaponComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        //var selfHealingEffect = new SelfHealingEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var streamWeapon = new StreamWeaponComponent().AddComponentGroup(turretGroupComponent);

                        var streamWeaponEnergy = new StreamWeaponEnergyComponent(DamageComponent.energyRechargeSpeedPerTimeProperty, DamageComponent.energyChargeSpeedPerTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { weaponComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(), damageEffect.SetGlobalComponentGroup(), streamWeapon.SetGlobalComponentGroup(), streamWeaponEnergy.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup(),
                            //selfHealingEffect.SetGlobalComponentGroup()
                        });
                    }
                    break;
                case "freeze":
                    var freezeComponent = new FreezeComponent
                    {
                        damagePerSecondProperty = float.Parse(serialObject["damagePerSecondProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        deltaTemperaturePerSecondProperty = float.Parse(serialObject["deltaTemperaturePerSecondProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        energyChargeSpeedProperty = float.Parse(serialObject["energyChargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        energyRechargeSpeedProperty = float.Parse(serialObject["energyRechargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        maxDamageDistanceProperty = float.Parse(serialObject["maxDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        minDamageDistanceProperty = float.Parse(serialObject["minDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        minDamagePercentProperty = float.Parse(serialObject["minDamagePercentProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                        temperatureLimitProperty = float.Parse(serialObject["temperatureLimitProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture)
                    };
                    freezeComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;
                    break;
                case "hammer":

                    break;
                case "isis":
                    if(true)//bypass visible zone
                    {
                        var isisComponent = new IsisComponent
                        {
                            damagePerSecondProperty = float.Parse(serialObject["damagePerSecondProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            decreaseFriendTemperatureProperty = float.Parse(serialObject["decreaseFriendTemperatureProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            energyChargeSpeedProperty = float.Parse(serialObject["energyChargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            energyRechargeSpeedProperty = float.Parse(serialObject["energyRechargeSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            healingProperty = float.Parse(serialObject["healingProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            increaseFriendTemperatureProperty = float.Parse(serialObject["increaseFriendTemperatureProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxShotDistanceProperty = float.Parse(serialObject["maxShotDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            selfHealingProperty = float.Parse(serialObject["selfHealingProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade,
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ConfigPath = new List<string> { serialObject.Path}
                        };
                        isisComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;
                        var DamageComponent = new IsisDamageComponent().UpdateComponent(isisComponent) as IsisDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = isisComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var selfHealingEffect = new SelfHealingEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var healingEffect = new HealingEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var streamWeapon = new StreamWeaponComponent().AddComponentGroup(turretGroupComponent);

                        var streamWeaponEnergy = new StreamWeaponEnergyComponent(DamageComponent.energyRechargeSpeedPerTimeProperty, DamageComponent.energyChargeSpeedPerTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { isisComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(), damageEffect.SetGlobalComponentGroup(), healingEffect.SetGlobalComponentGroup(), streamWeapon.SetGlobalComponentGroup(), streamWeaponEnergy.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup(),
                            selfHealingEffect.SetGlobalComponentGroup()
                        });
                    }
                    break;
                case "railgun":
                    if (true)//bypass visible zone
                    {
                        var railgunComponent = new RailgunComponent
                        {
                            impactProperty = float.Parse(serialObject["impactProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            kickbackProperty = float.Parse(serialObject["kickbackProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageProperty = float.Parse(serialObject["maxDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageProperty = float.Parse(serialObject["minDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            reloadTimeProperty = float.Parse(serialObject["reloadTimeProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            chargeTimeProperty = float.Parse(serialObject["chargeTimeProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            damageWeakeningByTargetProperty = float.Parse(serialObject["damageWeakeningByTargetProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade,
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ConfigPath = new List<string> { serialObject.Path }
                        };
                        railgunComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var DamageComponent = new RailgunDamageComponent().UpdateComponent(railgunComponent) as RailgunDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = railgunComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var energyDiscreteComponent = new DiscreteWeaponEnergyComponent(1f / railgunComponent.reloadTimeProperty, 1f, true).AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var impactComponent = new ImpactComponent(DamageComponent.impactProperty).AddComponentGroup(turretGroupComponent);

                        var kickBackComponent = new KickbackComponent(DamageComponent.kickbackProperty).AddComponentGroup(turretGroupComponent);

                        var weaponCooldwn = new WeaponCooldownComponent(DamageComponent.reloadTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { railgunComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(), damageEffect.SetGlobalComponentGroup(), impactComponent.SetGlobalComponentGroup(), kickBackComponent.SetGlobalComponentGroup(), weaponCooldwn.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyDiscreteComponent.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup() });
                    }
                    break;
                case "ricochet":

                    break;
                case "shaft":

                    break;
                case "smoky":
                    if (true)//bypass visible zone
                    {
                        var smokyComponent = new SmokyComponent
                        {
                            afterCriticalHitProbabilityProperty = float.Parse(serialObject["afterCriticalHitProbabilityProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            criticalDamageProperty = float.Parse(serialObject["criticalDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            criticalProbabilityDeltaProperty = float.Parse(serialObject["criticalProbabilityDeltaProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            impactProperty = float.Parse(serialObject["impactProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            kickbackProperty = float.Parse(serialObject["kickbackProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxCriticalProbabilityProperty = float.Parse(serialObject["maxCriticalProbabilityProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageDistanceProperty = float.Parse(serialObject["maxDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageProperty = float.Parse(serialObject["maxDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageDistanceProperty = float.Parse(serialObject["minDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamagePercentProperty = float.Parse(serialObject["minDamagePercentProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageProperty = float.Parse(serialObject["minDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            reloadTimeProperty = float.Parse(serialObject["reloadTimeProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            startCriticalProbabilityProperty = float.Parse(serialObject["startCriticalProbabilityProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade,
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ConfigPath = new List<string> { serialObject.Path}
                        };
                        smokyComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var DamageComponent = new SmokyDamageComponent().UpdateComponent(smokyComponent) as SmokyDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = smokyComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var energyDiscreteComponent = new DiscreteWeaponEnergyComponent(1f / smokyComponent.reloadTimeProperty, 1f, true).AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var impactComponent = new ImpactComponent(DamageComponent.impactProperty).AddComponentGroup(turretGroupComponent);

                        var kickBackComponent = new KickbackComponent(DamageComponent.kickbackProperty).AddComponentGroup(turretGroupComponent);

                        var weaponCooldwn = new WeaponCooldownComponent(DamageComponent.reloadTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { smokyComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(), damageEffect.SetGlobalComponentGroup(), impactComponent.SetGlobalComponentGroup(), kickBackComponent.SetGlobalComponentGroup(), weaponCooldwn.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyDiscreteComponent.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup() });
                    }
                    break;
                case "thunder":
                    if (true)//bypass visible zone
                    {
                        var thunderComponent = new ThunderComponent
                        {
                            impactProperty = float.Parse(serialObject["impactProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            kickbackProperty = float.Parse(serialObject["kickbackProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageDistanceProperty = float.Parse(serialObject["maxDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageProperty = float.Parse(serialObject["maxDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageDistanceProperty = float.Parse(serialObject["minDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamagePercentProperty = float.Parse(serialObject["minDamagePercentProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageProperty = float.Parse(serialObject["minDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            reloadTimeProperty = float.Parse(serialObject["reloadTimeProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            splashImpactProperty = float.Parse(serialObject["splashImpactProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade,
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ConfigPath = new List<string> { serialObject.Path }
                        };
                        thunderComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var DamageComponent = new ThunderDamageComponent().UpdateComponent(thunderComponent) as ThunderDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = thunderComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var energyDiscreteComponent = new DiscreteWeaponEnergyComponent(1f / thunderComponent.reloadTimeProperty, 1f, true).AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var impactComponent = new ImpactComponent(DamageComponent.impactProperty).AddComponentGroup(turretGroupComponent);

                        var kickBackComponent = new KickbackComponent(DamageComponent.kickbackProperty).AddComponentGroup(turretGroupComponent);

                        var thunderBattleConfig = ConstantService.GetByConfigPath("battle\\weapon\\thunder");
                        var splashWeaponComponent = new SplashWeaponComponent(
                            float.Parse(thunderBattleConfig.Deserialized["splashWeapon"]["minSplashDamagePercent"].ToString(), CultureInfo.InvariantCulture),
                            float.Parse(thunderBattleConfig.Deserialized["splashWeapon"]["radiusOfMaxSplashDamage"].ToString(), CultureInfo.InvariantCulture),
                            float.Parse(thunderBattleConfig.Deserialized["splashWeapon"]["radiusOfMinSplashDamage"].ToString(), CultureInfo.InvariantCulture)
                            ).AddComponentGroup(turretGroupComponent);

                        var weaponCooldwn = new WeaponCooldownComponent(DamageComponent.reloadTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { thunderComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(),  damageEffect.SetGlobalComponentGroup(), impactComponent.SetGlobalComponentGroup(), kickBackComponent.SetGlobalComponentGroup(), weaponCooldwn.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyDiscreteComponent.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup(),
                            splashWeaponComponent.SetGlobalComponentGroup()
                        });
                    }
                    break;
                case "vulcan":

                    break;
                case "twins":
                    if (true)//bypass visible zone
                    {
                        var twinsComponent = new TwinsComponent
                        {
                            impactProperty = float.Parse(serialObject["impactProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            kickbackProperty = float.Parse(serialObject["kickbackProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageDistanceProperty = float.Parse(serialObject["maxDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            maxDamageProperty = float.Parse(serialObject["maxDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageDistanceProperty = float.Parse(serialObject["minDamageDistanceProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamagePercentProperty = float.Parse(serialObject["minDamagePercentProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            minDamageProperty = float.Parse(serialObject["minDamageProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            reloadTimeProperty = float.Parse(serialObject["reloadTimeProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            bulletRadiusProperty = float.Parse(serialObject["bulletRadiusProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            bulletSpeedProperty = float.Parse(serialObject["bulletSpeedProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ComponentGrade = Grade,
                            turretSpeedRotationProperty = float.Parse(serialObject["turretSpeedRotationProperty"]["initialValue"].ToString(), CultureInfo.InvariantCulture),
                            ConfigPath = new List<string> { serialObject.Path }
                        };
                        twinsComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var DamageComponent = new TwinsDamageComponent().UpdateComponent(twinsComponent) as TwinsDamageComponent;
                        DamageComponent.ComponentGroups[TurretGroupComponent.Id] = turretGroupComponent;

                        var characteristicTransformer = new CharacteristicTransformersComponent().AddComponentGroup(turretGroupComponent) as CharacteristicTransformersComponent;
                        characteristicTransformer.sourceDamageComponent = twinsComponent;
                        characteristicTransformer.damageComponent = DamageComponent;

                        var effectsAggregator = new EffectsAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        //var resistanceAggregator = new ResistanceAggregatorComponent().AddComponentGroup(turretGroupComponent);

                        var damageEffect = new DamageEffect(entity, DamageComponent).AddComponentGroup(turretGroupComponent);

                        var energyDiscreteComponent = new DiscreteWeaponEnergyComponent(1f, 1f, true) {reloadTime = twinsComponent.reloadTimeProperty }.AddComponentGroup(turretGroupComponent);

                        var energyComponent = new WeaponEnergyComponent(1f).AddComponentGroup(turretGroupComponent);

                        var impactComponent = new ImpactComponent(DamageComponent.impactProperty).AddComponentGroup(turretGroupComponent);

                        var kickBackComponent = new KickbackComponent(DamageComponent.kickbackProperty).AddComponentGroup(turretGroupComponent);

                        var weaponCooldwn = new WeaponCooldownComponent(DamageComponent.reloadTimeProperty).AddComponentGroup(turretGroupComponent);

                        var weaponRotation = new WeaponRotationComponent(DamageComponent.turretSpeedRotationProperty).AddComponentGroup(turretGroupComponent);

                        entity.AddComponentsSilent(new ECSComponent[] { twinsComponent.SetGlobalComponentGroup(), DamageComponent.SetGlobalComponentGroup(), characteristicTransformer.SetGlobalComponentGroup(), effectsAggregator.SetGlobalComponentGroup(), damageEffect.SetGlobalComponentGroup(), impactComponent.SetGlobalComponentGroup(), kickBackComponent.SetGlobalComponentGroup(), weaponCooldwn.SetGlobalComponentGroup(), weaponRotation.SetGlobalComponentGroup(), energyDiscreteComponent.SetGlobalComponentGroup(), energyComponent.SetGlobalComponentGroup() });
                    }
                    break;
                case "gauss":

                    break;
                case "tesla":

                    break;
            }
        }

        private void ColormapComponentsUpdate(ECSEntity entity, ConfigObj configObj)
        {
            var colormapGroupComponent = new ColormapGroupComponent();
            var resistsList = configObj.Deserialized["weaponResists"].ToList();
            entity.AddComponent(new ResistanceAggregatorComponent().AddComponentGroup(colormapGroupComponent).SetGlobalComponentGroup());
            entity.AddComponentSilent(new ColormapComponent
            {
                ConfigPath = new List<string>
            { configObj.Path }
            }.AddComponentGroup(colormapGroupComponent));
            foreach (var resist in resistsList)
            {
                switch (resist["weaponResist"].ToString())
                {
                    case "garage\\weapon\\flamethrower":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = FlamethrowerDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\freeze":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = FreezeDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\hammer":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = HammerDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\isis":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = IsisDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\railgun":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = RailgunDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\ricochet":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = RicochetDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\shaft":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = ShaftDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\smoky":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = SmokyDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\thunder":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = ThunderDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\vulcan":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId =  VulcanDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\twins":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = TwinsDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\gauss":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = GaussDamageComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "battle\\creature\\mine":
                        entity.AddComponent(new DamageResistComponent() { DamageComponentId = MineCreatureComponent.Id, ConfigPath = new List<string> { configObj.Path }, WeaponResistPercent = int.Parse(resist["resistPercents"].ToString(), CultureInfo.InvariantCulture) }.AddComponentGroup(colormapGroupComponent));
                        break;
                    case "garage\\weapon\\tesla":

                        break;
                }
                entity.TryRemoveComponent(DamageResistComponent.Id);
            }
        }

        private void TurretModulesComponentsUpdate(ECSEntity entity, ConfigObj configObj)
        {

        }

        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
