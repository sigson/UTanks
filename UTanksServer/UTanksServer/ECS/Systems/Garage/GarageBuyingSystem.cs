using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Events.Garage;
using UTanksServer.ECS.Templates.Battle;
using UTanksServer.ECS.Templates.User;
using UTanksServer.Extensions;
using UTanksServer.Services;

namespace UTanksServer.ECS.Systems.Garage
{
    public class GarageBuyingSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(GarageBuyItemEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    var typedEvent = (GarageBuyItemEvent)Event;
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
                    var GarageDB = (UserGarageDBComponent)entity.GetComponent(UserGarageDBComponent.Id);
                    bool changeFlag = false;
                    GarageDB.locker.EnterWriteLock();
                    try
                    {
                        changeFlag = BuyItem(entity, GarageDB, typedEvent, configObj);
                    }
                    finally
                    {
                        GarageDB.locker.ExitWriteLock();
                    }
                    if(changeFlag)
                    {
                        //ManagerScope.eventManager.OnEventAdd(new WeaponChangeEvent(){EntityOwnerId = typedEvent.EntityOwnerId, Grade = typedEvent.Grade, ItemId = typedEvent.ItemId, ConfigPath = typedEvent.ConfigPath, SkinConfigPath = typedEvent.SkinConfigPath });
                    }
                    else
                    {
                        return null;
                        //ddos event
                    }
                    return null;
                }
            });
        }

        public bool BuyItem(ECSEntity userEntity, UserGarageDBComponent GarageDB, GarageBuyItemEvent weaponBuyEvent, ConfigObj configObj, float outSell = 0f)
        {
            switch (configObj.HeadLibName)
            {
                case "hull":
                    if (true)
                    {
                        Types.Hull findedHull = null;
                        foreach (var hull in GarageDB.garage.Hulls)
                        {
                            if (hull.PathName == configObj.Path)
                            {
                                findedHull = hull;
                            }
                            if (hull.PathName == configObj.Path && (hull.Grade == weaponBuyEvent.Grade || weaponBuyEvent.Grade < hull.Grade))
                            {
                                //ddos event
                                return false;
                            }
                        }
                        var shopObject = GlobalGameDataConfig.GarageShop.garageShop.Hulls.Where(x => x.PathName == weaponBuyEvent.ConfigPath && x.Grade == weaponBuyEvent.Grade).ToList();
                        List<Types.Skin> defaultSkinPath = null;
                        if (shopObject.Count != 0)
                            defaultSkinPath = new List<Types.Skin>(shopObject[0].Skins);
                        else
                            return false;//ddos
                        var serialObject = configObj.Deserialized["grades"].ToList()[weaponBuyEvent.Grade];
                        var userRank = userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent;
                        var userCrystalls = userEntity.GetComponent(UserCrystalsComponent.Id) as UserCrystalsComponent;
                        //userRank.locker.EnterReadLock();

                        if (userRank.Rank >= int.Parse(serialObject["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString(), CultureInfo.InvariantCulture))
                        {

                            var itemStandartPrice = int.Parse(serialObject["priceItem"]["price"].ToString(), CultureInfo.InvariantCulture);
                            var itemPrice = itemStandartPrice;
                            float itemDiscount;
                            if (outSell != 0f)
                            {
                                itemPrice = (int)(itemStandartPrice * outSell / 100);
                            }
                            else if (GlobalGameDataConfig.DiscountList.Discounts.TryGetValue(configObj.Path, out itemDiscount))
                            {
                                itemPrice = (int)(itemStandartPrice * itemDiscount / 100);
                            }

                            

                            userCrystalls.locker.EnterWriteLock();
                            if (userCrystalls.UserCrystals >= itemPrice)
                            {

                                userCrystalls.UserCrystals -= itemPrice;
                                //GarageDB.locker.EnterWriteLock();
                                //GarageDB.locker.ExitWriteLock();

                                if (findedHull != null)
                                {
                                    GarageDB.garage.Hulls.Remove(findedHull);
                                    if (GarageDB.selectedEquipment.Hulls.Where(x => x.PathName == findedHull.PathName).Count() > 0)
                                    {
                                        defaultSkinPath.ForEach(xdefaultSkinPath =>
                                        {
                                            if (!(findedHull.Skins.Where(x => x.SkinPathName == xdefaultSkinPath.SkinPathName).Count() > 0))
                                                findedHull.Skins.Add(new Types.Skin()
                                                {
                                                    SkinPathName = xdefaultSkinPath.SkinPathName,
                                                    Equiped = false
                                                });
                                        });
                                        GarageDB.selectedEquipment.Hulls.RemoveAt(0);
                                        GarageDB.selectedEquipment.Hulls.Add(new Types.Hull { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = new List<Types.Skin>() { findedHull.Skins.Where(x => x.Equiped).ToList()[0] } });
                                    }
                                    GarageDB.garage.Hulls.Add(new Types.Hull { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = new List<Types.Skin>(findedHull.Skins) });
                                }
                                else
                                {
                                    GarageDB.garage.Hulls.Add(new Types.Hull { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = defaultSkinPath });
                                }
                            }
                            else
                            {
                                userCrystalls.locker.ExitWriteLock();
                                return false;
                                //ddos event
                            }
                            userCrystalls.locker.ExitWriteLock();
                            userCrystalls.MarkAsChanged();
                            GarageDB.MarkAsChanged();
                        }
                        else
                        {
                            return false;
                            //ddos event
                        }
                        return true;
                        break;
                    }
                case "weapon":
                    if (true)
                    {
                        Types.Turret findedTurret = null;
                        foreach (var Turret in GarageDB.garage.Turrets)
                        {
                            if (Turret.PathName == configObj.Path)
                            {
                                findedTurret = Turret;
                            }
                            if (Turret.PathName == configObj.Path && (Turret.Grade == weaponBuyEvent.Grade || weaponBuyEvent.Grade < Turret.Grade))
                            {
                                return false;
                                //ddos event
                            }
                        }
                        var shopObject = GlobalGameDataConfig.GarageShop.garageShop.Turrets.Where(x => x.PathName == weaponBuyEvent.ConfigPath && x.Grade == weaponBuyEvent.Grade).ToList();
                        List<Types.Skin> defaultSkinPath = null;
                        if (shopObject.Count != 0)
                            defaultSkinPath = new List<Types.Skin>(shopObject[0].Skins);
                        else
                            return false;//ddos
                        var serialObject = configObj.Deserialized["grades"].ToList()[weaponBuyEvent.Grade];
                        var userRank = userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent;
                        var userCrystalls = userEntity.GetComponent(UserCrystalsComponent.Id) as UserCrystalsComponent;
                        //userRank.locker.EnterReadLock();

                        if (userRank.Rank >= int.Parse(serialObject["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString(), CultureInfo.InvariantCulture))
                        {
                            var itemStandartPrice = int.Parse(serialObject["priceItem"]["price"].ToString(), CultureInfo.InvariantCulture);
                            var itemPrice = itemStandartPrice;
                            float itemDiscount;
                            if(outSell != 0f)
                            {
                                itemPrice = (int)(itemStandartPrice * outSell / 100);
                            }
                            else if(GlobalGameDataConfig.DiscountList.Discounts.TryGetValue(configObj.Path, out itemDiscount))
                            {
                                itemPrice = (int)(itemStandartPrice * itemDiscount / 100);
                            }
                            userCrystalls.locker.EnterWriteLock();
                            if (userCrystalls.UserCrystals >= itemPrice)
                            {

                                userCrystalls.UserCrystals -= itemPrice;
                                //GarageDB.locker.EnterWriteLock();
                                if (findedTurret != null)
                                {
                                    GarageDB.garage.Turrets.Remove(findedTurret);
                                    if (GarageDB.selectedEquipment.Turrets.Where(x => x.PathName == findedTurret.PathName).Count() > 0)
                                    {
                                        defaultSkinPath.ForEach(xdefaultSkinPath =>
                                        {
                                            if (!(findedTurret.Skins.Where(x => x.SkinPathName == xdefaultSkinPath.SkinPathName).Count() > 0))
                                                findedTurret.Skins.Add(new Types.Skin() {
                                                    SkinPathName = xdefaultSkinPath.SkinPathName,
                                                    Equiped = false
                                                });
                                        });
                                        GarageDB.selectedEquipment.Turrets.RemoveAt(0);
                                        GarageDB.selectedEquipment.Turrets.Add(new Types.Turret { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = new List<Types.Skin>() { findedTurret.Skins.Where(x => x.Equiped).ToList()[0] } });
                                    }
                                    GarageDB.garage.Turrets.Add(new Types.Turret { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = new List<Types.Skin>(findedTurret.Skins) });
                                }
                                else
                                {
                                    GarageDB.garage.Turrets.Add(new Types.Turret { Grade = weaponBuyEvent.Grade, PathName = configObj.Path, Skins = defaultSkinPath });
                                }
                                //GarageDB.locker.ExitWriteLock();
                            }
                            else
                            {
                                userCrystalls.locker.ExitWriteLock();
                                return false;
                                //ddos event
                            }
                            userCrystalls.locker.ExitWriteLock();
                            userCrystalls.MarkAsChanged();
                            GarageDB.MarkAsChanged();
                        }
                        else
                        {
                            return false;
                            //ddos event
                        }
                        return true;
                        break;
                    }
                case "turretmodules" or "hullmodules":
                    if (true)
                    {
                        foreach (var turretModules in GarageDB.garage.Modules)
                        {
                            if (turretModules.PathName == configObj.Path)
                            {
                                return false;
                            }
                        }
                        break;
                    }
                case "colormap":                  
                    if (true)
                    {
                        foreach (var colormap in GarageDB.garage.Colormaps)
                        {
                            if (colormap.PathName == configObj.Path)
                            {
                                return false;
                            }
                        }
                        var userRank = userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent;
                        var userCrystalls = userEntity.GetComponent(UserCrystalsComponent.Id) as UserCrystalsComponent;
                        //userRank.locker.EnterReadLock();

                        if (userRank.Rank >= int.Parse(configObj.Deserialized["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString(), CultureInfo.InvariantCulture))
                        {

                            var itemStandartPrice = int.Parse(configObj.Deserialized["priceItem"]["price"].ToString(), CultureInfo.InvariantCulture);
                            var itemPrice = itemStandartPrice;
                            float itemDiscount;
                            if (outSell != 0f)
                            {
                                itemPrice = (int)(itemStandartPrice * outSell / 100);
                            }
                            else if (GlobalGameDataConfig.DiscountList.Discounts.TryGetValue(configObj.Path, out itemDiscount))
                            {
                                itemPrice = (int)(itemStandartPrice * itemDiscount / 100);
                            }

                            userCrystalls.locker.EnterWriteLock();
                            if (userCrystalls.UserCrystals >= itemPrice)
                            {
                                userCrystalls.UserCrystals -= itemPrice;
                                //GarageDB.locker.EnterWriteLock();
                                GarageDB.garage.Colormaps.Add(new Types.Colormap { PathName = configObj.Path });
                                //GarageDB.locker.ExitWriteLock();
                            }
                            else
                            {
                                userCrystalls.locker.ExitWriteLock();
                                return false;
                                //ddos event
                            }
                            userCrystalls.locker.ExitWriteLock();
                            userCrystalls.MarkAsChanged();
                            GarageDB.MarkAsChanged();
                        }
                        else
                        {
                            return false;
                            //ddos event
                        }
                        return true;
                        break;
                    }
                case "supplies":
                    if (true)
                    {
                        if(weaponBuyEvent.Count <= 0)
                        {
                            return false;
                            //ddos;
                        }
                        Types.Supply supplyPresentedInGarage = null;
                        foreach (var supply in GarageDB.garage.Supplies)
                        {
                            if (supply.PathName == configObj.Path)
                            {
                                supplyPresentedInGarage = supply;
                            }
                        }
                        var userRank = userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent;
                        var userCrystalls = userEntity.GetComponent(UserCrystalsComponent.Id) as UserCrystalsComponent;
                        //userRank.locker.EnterReadLock();

                        if (userRank.Rank >= int.Parse(configObj.Deserialized["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString(), CultureInfo.InvariantCulture))
                        {

                            var itemStandartPrice = int.Parse(configObj.Deserialized["priceItem"]["price"].ToString(), CultureInfo.InvariantCulture);
                            var itemPrice = itemStandartPrice;
                            float itemDiscount;
                            if (outSell != 0f)
                            {
                                itemPrice = (int)(itemStandartPrice * outSell / 100);
                            }
                            else if (GlobalGameDataConfig.DiscountList.Discounts.TryGetValue(configObj.Path, out itemDiscount))
                            {
                                itemPrice = (int)(itemStandartPrice * itemDiscount / 100);
                            }

                            userCrystalls.locker.EnterWriteLock();
                            if (userCrystalls.UserCrystals >= itemPrice * weaponBuyEvent.Count)
                            {
                                userCrystalls.UserCrystals -= itemPrice * weaponBuyEvent.Count;
                                //GarageDB.locker.EnterWriteLock();
                                if (supplyPresentedInGarage == null)
                                {
                                    GarageDB.garage.Supplies.Add(new Types.Supply { PathName = configObj.Path, Count = weaponBuyEvent.Count });
                                }
                                else
                                {
                                    supplyPresentedInGarage.Count += weaponBuyEvent.Count;
                                }
                                //GarageDB.locker.ExitWriteLock();
                            }
                            else
                            {
                                userCrystalls.locker.ExitWriteLock();
                                return false;
                                //ddos event
                            }
                            userCrystalls.locker.ExitWriteLock();
                            userCrystalls.MarkAsChanged();
                            GarageDB.MarkAsChanged();
                            if(userEntity.HasComponent(TankInBattleComponent.Id))
                            {
                                UpdateSupplyCountEvent updateSupplyCountEvent = new UpdateSupplyCountEvent() { EntityOwnerId = userEntity.instanceId };
                                BattleUserTemplate.GenerateBattleUserEquipment(userEntity, userEntity.GetComponent<BattleOwnerComponent>().Battle, updateSupplyCountEvent);
                                if(updateSupplyCountEvent.InBattleSupply.Count > 0 || updateSupplyCountEvent.SyncSupply.Count > 0)
                                {
                                    userEntity.GetComponent<UserSocketComponent>().Socket.emit(updateSupplyCountEvent.PackToNetworkPacket());
                                }
                            }
                        }
                        else
                        {
                            return false;
                            //ddos event
                        }
                        return true;
                        break;
                    }
                case "equipmentkit":
                    //for(buy equip with kit outSell)
                    break;
                case "boosters":
                    if (true)
                    {
                        if (weaponBuyEvent.Count <= 0)
                        {
                            return false;
                            //ddos;
                        }
                        Types.Supply supplyPresentedInGarage = null;
                        foreach (var supply in GarageDB.garage.Supplies)
                        {
                            if (supply.PathName == configObj.Path)
                            {
                                supplyPresentedInGarage = supply;
                            }
                        }
                        var userRank = userEntity.GetComponent(UserRankComponent.Id) as UserRankComponent;
                        var userCrystalls = userEntity.GetComponent(UserCrystalsComponent.Id) as UserCrystalsComponent;
                        //userRank.locker.EnterReadLock();

                        if (userRank.Rank >= int.Parse(configObj.Deserialized["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString(), CultureInfo.InvariantCulture))
                        {

                            var itemStandartPrice = int.Parse(configObj.Deserialized["priceItem"]["price"].ToString(), CultureInfo.InvariantCulture);
                            var itemPrice = itemStandartPrice;
                            float itemDiscount;
                            if (outSell != 0f)
                            {
                                itemPrice = (int)(itemStandartPrice * outSell / 100);
                            }
                            else if (GlobalGameDataConfig.DiscountList.Discounts.TryGetValue(configObj.Path, out itemDiscount))
                            {
                                itemPrice = (int)(itemStandartPrice * itemDiscount / 100);
                            }

                            userCrystalls.locker.EnterWriteLock();
                            if (userCrystalls.UserCrystals >= itemPrice * weaponBuyEvent.Count)
                            {
                                userCrystalls.UserCrystals -= itemPrice * weaponBuyEvent.Count;
                                if (configObj.Path == @"garage\boosters\scorepack")
                                {
                                    UserTemplate.ScoreUpdate(userEntity, configObj.Deserialized["oneTimeScoreUpper"]["score"].ToObject<int>() * weaponBuyEvent.Count);
                                }
                                //GarageDB.locker.EnterWriteLock();
                                //if (supplyPresentedInGarage == null)
                                //{
                                //    GarageDB.garage.Supplies.Add(new Types.Supply { PathName = configObj.Path, Count = weaponBuyEvent.Count });
                                //}
                                //else
                                //{
                                //    supplyPresentedInGarage.Count += weaponBuyEvent.Count;
                                //}
                                //GarageDB.locker.ExitWriteLock();
                            }
                            else
                            {
                                userCrystalls.locker.ExitWriteLock();
                                return false;
                                //ddos event
                            }
                            userCrystalls.locker.ExitWriteLock();
                            userCrystalls.MarkAsChanged();
                            //GarageDB.MarkAsChanged();
                        }
                        else
                        {
                            return false;
                            //ddos event
                        }
                        return true;
                        break;
                    }
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
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { GarageBuyItemEvent.Id }, IValues = { 0 } }.Upd();
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
