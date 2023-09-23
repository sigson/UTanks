using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Garage;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ComponentsGroup.Garage;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Garage;
using UTanksServer.ECS.Templates;
using UTanksServer.Extensions;
using UTanksServer.Services;

namespace UTanksServer.ECS.Systems.Garage
{
    class ChangeEquipmentSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(WeaponChangeEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    var typedEvent = (WeaponChangeEvent)Event;
                    ConfigObj configObj = null;
                    if(typedEvent.ConfigPath == null || !ConstantService.ConstantDB.TryGetValue(typedEvent.ConfigPath, out configObj))
                    {
                        return null;
                        //ddos event
                    }
                    ConfigObj configSkinObj = null;
                    if(typedEvent.SkinConfigPath == null || !ConstantService.ConstantDB.TryGetValue(typedEvent.SkinConfigPath, out configSkinObj))
                    {
                        //return null;
                        //ddos event
                    }

                    var entity = ManagerScope.entityManager.EntityStorage[Event.EntityOwnerId];
                    if(entity.HasComponent(InBattleChangeWeaponTimeoutComponent.Id))
                    {
                        return null;
                        //ddos event
                    }
                    var GarageDB = (UserGarageDBComponent)entity.GetComponent(UserGarageDBComponent.Id);
                    bool changeFlag = false;
                    GarageDB.locker.EnterWriteLock();
                    try
                    {
                        changeFlag = ChangeWeapon(GarageDB, typedEvent, configObj, configSkinObj);
                    }
                    finally
                    {
                        GarageDB.locker.ExitWriteLock();
                    }
                    if(changeFlag)
                    {
                        entity.ChangeComponent(GarageDB);
                        //entity.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id).UserCrystals += 4000;
                        //entity.ChangeComponent(entity.GetComponent<UserCrystalsComponent>(UserCrystalsComponent.Id));
                        //new EquipmentTemplate().UpdateEntity(entity, configObj, typedEvent);
                        entity.entityComponents.RegisterAllComponents();
                        if(entity.HasComponent(TankInBattleComponent.Id))
                        {
                            entity.AddComponent(new InBattleChangeEquipmentComponent(5f));
                            entity.AddComponent(new InBattleChangeWeaponTimeoutComponent());
                        }

                    }
                    else
                    {
                        return null;
                        //ddos event
                    }
                    
                    
                    UpdateEventWatcher(Event);
                    return null;
                }
            });
        }

        public bool ChangeWeapon(UserGarageDBComponent GarageDB, WeaponChangeEvent weaponChangeEvent, ConfigObj configObj, ConfigObj configSkinObj)
        {
            //if(GarageDB.garage.
            switch(configObj.HeadLibName)
            {
                case "hull":
                    if (configSkinObj == null)
                        return false;//ddos
                    foreach (var hull in GarageDB.garage.Hulls) {
                        if (hull.PathName == configObj.Path && hull.Grade == weaponChangeEvent.Grade) {
                            var garageHullSkin = hull.Skins.Where(x => x.SkinPathName == weaponChangeEvent.SkinConfigPath);
                            if (garageHullSkin.Count() > 0)
                            {
                                GarageDB.selectedEquipment.Hulls.Clear();
                                var newHull = new Types.Hull();
                                newHull.Grade = hull.Grade;
                                newHull.PathName = hull.PathName;
                                newHull.Skins = new List<Types.Skin>();
                                //newHull.SkinPathName.Clear();
                                var newSkin = new Types.Skin()
                                {
                                    SkinPathName = weaponChangeEvent.SkinConfigPath,
                                    Equiped = true
                                };
                                newHull.Skins.Add(newSkin);
                                hull.Skins.ForEach(x => x.Equiped = false);
                                garageHullSkin.ToList()[0].Equiped = true;
                                GarageDB.selectedEquipment.Hulls.Add(newHull);
                                return true;
                            }
                        }
                    }
                    break;
                case "weapon":
                    if (configSkinObj == null)
                        return false;//ddos
                    foreach (var turret in GarageDB.garage.Turrets) {
                        if (turret.PathName == configObj.Path && turret.Grade == weaponChangeEvent.Grade)
                        {
                            var garageTurretSkin = turret.Skins.Where(x => x.SkinPathName == weaponChangeEvent.SkinConfigPath);
                            if (garageTurretSkin.Count() > 0)
                            {
                                GarageDB.selectedEquipment.Turrets.Clear();
                                var newTurret = new Types.Turret();
                                newTurret.Grade = turret.Grade;
                                newTurret.PathName = turret.PathName;
                                newTurret.Skins = new List<Types.Skin>();
                                //newHull.SkinPathName.Clear();
                                var newSkin = new Types.Skin()
                                {
                                    SkinPathName = weaponChangeEvent.SkinConfigPath,
                                    Equiped = true
                                };
                                //newHull.SkinPathName.Clear();
                                newTurret.Skins.Add(newSkin);
                                turret.Skins.ForEach(x => x.Equiped = false);
                                garageTurretSkin.ToList()[0].Equiped = true;
                                GarageDB.selectedEquipment.Turrets.Add(newTurret);
                                return true;
                            }
                        }
                    }
                    break;
                case "turretmodules" or "hullmodules":
                    foreach (var turretModules in GarageDB.garage.Modules) { 
                        if (turretModules.PathName == configObj.Path)
                        {
                            GarageDB.selectedEquipment.Modules.Clear();
                            GarageDB.selectedEquipment.Modules.Add(turretModules);
                            return true;
                        }
                    }
                    break;
                case "colormap":
                    foreach (var colormap in GarageDB.garage.Colormaps) { 
                        if (colormap.PathName == configObj.Path)
                        {
                            GarageDB.selectedEquipment.Colormaps.Clear();
                            GarageDB.selectedEquipment.Colormaps.Add(colormap);
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { }, IValues = { } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { WeaponChangeEvent.Id }, IValues = { 0 } }.Upd();
        }

        public override void Run(long[] entities)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            throw new NotImplementedException();
        }
    }
}
