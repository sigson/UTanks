using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.DBResources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.Extensions;
using UTanksClient.Services;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UnityExtend;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Types;
using UTanksClient.ClassExtensions;

namespace SecuredSpace.UI.GameUI
{
    public class GarageUIHandler : MonoBehaviour
    {
        public GarageTankPreviewHandler tankPreviewHandler;
        public GarageItemHandler itemHandler;

        public SerializableDictionary<string, Sprite> GarageItemBackgroundStates = new SerializableDictionary<string, Sprite>();

        public GameObject SkinWindow;
        public GameObject SkinWindowCanvas;
        public GameObject SkinElementExample;
        public GarageDeviceListElement GarageDeviceListElementExample;
        public List<GarageDeviceListElement> Devices = new List<GarageDeviceListElement>();

        
        public GameObject GarageItemsList;
        public Dictionary<string, Dictionary<string, GarageListItemElement>> GarageItemsListRepresentation = new Dictionary<string, Dictionary<string, GarageListItemElement>>();
        public Dictionary<string, GarageListItemElement> GarageItemsDB = new Dictionary<string, GarageListItemElement>();
        public GameObject GarageItemsListElementTemplate;

        private string nowCategoryValue = "";
        [NonSerialized]public bool AdditionCategory = false;
        public string NowCategory
        {
            get
            {
                return nowCategoryValue;
            }
            set
            {
                if(nowCategoryValue == "" || value == "")
                {
                    ReprezentationCategoryPrepare();
                }
                else if(!AdditionCategory)
                    GarageItemsListRepresentation[nowCategoryValue].ForEach(x => x.Value.gameObject.SetActive(false));
                if (value == "")
                    return;
                
                GarageItemsListRepresentation[value].ForEach(x => x.Value.gameObject.SetActive(true));
                if(nowCategoryValue != value)
                {
                    var rect = GarageItemsListElementTemplate.transform.parent.GetComponent<RectTransform>();
                    rect.localPosition = rect.localPosition.SetEx(0);
                }
                nowCategoryValue = value;
            }
        }
        public UnityExtend.SerializableDictionary<string, GarageItemCategorySwitcher> Categories = new UnityExtend.SerializableDictionary<string, GarageItemCategorySwitcher>();

        void Start()
        {
            GarageItemsListElementTemplate.SetActive(false);
        }

        #region Skin

        public void ShowDeviceWindow()
        {
            SkinWindow.SetActive(true);
            var selectedItem = itemHandler.selectedItem;
            foreach(var skinPath in selectedItem.SkinItemPath)
            {
                var skinElement = Instantiate(SkinElementExample, SkinWindowCanvas.transform).GetComponent<GarageDeviceListElement>();
                skinElement.DevicePath = skinPath.SkinPathName;
                skinElement.Grade = selectedItem.Grade;
                skinElement.ItemPath = selectedItem.ItemPath;
                skinElement.itemResources = selectedItem.resourcesObj;
                skinElement.Initialize();
                skinElement.gameObject.SetActive(true);
                Devices.Add(skinElement);
            }
        }

        public void SelectDevice(string selectedSkinPath)
        {
            var selectedItem = itemHandler.selectedItem;
            var weaponBuyEvent = new WeaponChangeEvent() { ConfigPath = selectedItem.ItemPath, Grade = selectedItem.Grade, SkinConfigPath = selectedSkinPath };
            TaskEx.RunAsync(() => {
                ClientNetworkService.instance.Socket.emit<GameDataEvent>(weaponBuyEvent.PackToNetworkPacket());
            });
        }

        public void HideDeviceWindow()
        {
            //var selectedItem = OperationItemButton.GetComponent<GarageOperationsHandler>().selectedItem;
            Devices.ForEach(x => Destroy(x.gameObject));
            Devices.Clear();
            SkinWindow.SetActive(false);
        }

        #endregion

        private void ReprezentationCategoryPrepare()
        {
            Categories.ForEach(x => {
                if (!GarageItemsListRepresentation.ContainsKey(x.Key))
                    GarageItemsListRepresentation[x.Key] = new Dictionary<string, GarageListItemElement>();
            });
        }

        public void GarageUpdate(UserGarageDBComponent userGarageDBComponent)
        {
            tankPreviewHandler.UpdateTankPreview(userGarageDBComponent);

            #region userold
            ////string selectedPath = "";
            ////foreach (var element in PlayerItemsListRepresentation.Values)
            ////{
            ////    if (element.Selected)
            ////    {
            ////        selectedPath = element.ItemPath;
            ////    }
            ////    Destroy(element.gameObject);
            ////}
            ////PlayerItemsListRepresentation.Clear();
            ////foreach (var element in GarageItemsListRepresentation.Values)
            ////{
            ////    if (element.Selected)
            ////    {
            ////        selectedPath = element.ItemPath;
            ////    }
            ////    Destroy(element.gameObject);
            ////}
            ////GarageItemsListRepresentation.Clear();

            ////userGarageDBComponent.garage.Colormaps = userGarageDBComponent.garage.Colormaps.OrderBy(x => int.Parse(ConstantService.instance.ConstantDB[x.PathName].Deserialized["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString())).ToList();

            ////foreach (var supply in userGarageDBComponent.garage.Supplies)
            ////{
            ////    var constantInfo = ConstantService.instance.ConstantDB[supply.PathName];
            ////    var preview = ClientInitService.instance.ItemResourcesDBOld.UIObjectsDB["ItemUI"]["Supplies"].Preview[supply.PathName];
            ////    var newElement = Instantiate(PlayerItemsListElementTemplate, PlayerItemsList.transform);
            ////    var itemScript = newElement.GetComponent<GarageListItemElement>();
            ////    PlayerItemsListRepresentation.Add(supply.PathName, itemScript);
            ////    itemScript.ItemPath = supply.PathName;
            ////    itemScript.constantObj = constantInfo;
            ////    itemScript.ItemCount = supply.Count;
            ////    itemScript.UserItem = true;
            ////    itemScript.Preview = Sprite.Create(preview, new Rect(0, 0, preview.width, preview.height), new Vector2());
            ////    itemScript.Price = int.Parse(constantInfo.Deserialized["priceItem"]["price"].ToString());
            ////    var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
            ////    itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
            ////    //if (userGarageDBComponent.selectedEquipment.Turret.Contains(turret))
            ////    //{
            ////    //    itemScript.Equiped = true;
            ////    //}
            ////    itemScript.gameObject.SetActive(true);
            ////    itemScript.UpdateData();
            ////}
            ////foreach (var supply in userGarageDBComponent.garage.ModuleWithTimeExpiration)
            ////{

            ////}
            ////foreach (var turret in userGarageDBComponent.garage.Turrets)
            ////{
            ////    var constantInfo = ConstantService.instance.ConstantDB[turret.PathName];
            ////    //var resourcesData = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[turret.PathName][turret.Grade.ToString()];
            ////    var resourcesData = new ItemCard();
            ////    turret.Skins.ForEach(x => {
            ////        if (!ConstantService.instance.GetByConfigPath(x.SkinPathName).Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            ////        {
            ////            resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName + "\\" + turret.Grade.ToString()).FillChildContentToItem(x.SkinPathName + "\\");
            ////        }
            ////        else
            ////        {
            ////            resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName).FillChildContentToItem(x.SkinPathName + "\\");
            ////        }
            ////    });
            ////    var newElement = Instantiate(PlayerItemsListElementTemplate, PlayerItemsList.transform);
            ////    var itemScript = newElement.GetComponent<GarageListItemElement>();
            ////    PlayerItemsListRepresentation.Add(turret.PathName, itemScript);
            ////    itemScript.ItemPath = turret.PathName;
            ////    itemScript.SkinItemPath = turret.Skins;
            ////    itemScript.ItemGradeString = "M" + turret.Grade.ToString();
            ////    itemScript.resourcesObj = resourcesData;
            ////    itemScript.constantObj = constantInfo;
            ////    itemScript.Grade = turret.Grade;
            ////    itemScript.ItemCount = -1;
            ////    itemScript.UserItem = true;
            ////    try
            ////    {
            ////        itemScript.NextGradePrice = int.Parse(constantInfo.Deserialized["grades"][turret.Grade + 1]["priceItem"]["price"].ToString());
            ////    }
            ////    catch { }

            ////    var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
            ////    itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
            ////    if (userGarageDBComponent.selectedEquipment.Turrets.Contains(turret))
            ////    {
            ////        itemScript.Equiped = true;
            ////    }
            ////    itemScript.gameObject.SetActive(true);
            ////    itemScript.UpdateData();
            ////}
            ////foreach (var hull in userGarageDBComponent.garage.Hulls)
            ////{
            ////    var constantInfo = ConstantService.instance.ConstantDB[hull.PathName];
            ////    //var resourcesData = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[hull.PathName][hull.Grade.ToString()];
            ////    var resourcesData = new ItemCard();
            ////    hull.Skins.ForEach(x => {
            ////        if (!ConstantService.instance.GetByConfigPath(x.SkinPathName).Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            ////        {
            ////            resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName + "\\" + hull.Grade.ToString()).FillChildContentToItem(x.SkinPathName + "\\");
            ////        }
            ////        else
            ////        {
            ////            resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName).FillChildContentToItem(x.SkinPathName + "\\");
            ////        }
            ////    });
            ////    var newElement = Instantiate(PlayerItemsListElementTemplate, PlayerItemsList.transform);
            ////    var itemScript = newElement.GetComponent<GarageListItemElement>();
            ////    PlayerItemsListRepresentation.Add(hull.PathName, itemScript);
            ////    itemScript.ItemPath = hull.PathName;
            ////    itemScript.SkinItemPath = hull.Skins;
            ////    itemScript.ItemGradeString = "M" + hull.Grade.ToString();
            ////    itemScript.resourcesObj = resourcesData;
            ////    itemScript.constantObj = constantInfo;
            ////    itemScript.Grade = hull.Grade;
            ////    itemScript.ItemCount = -1;
            ////    itemScript.UserItem = true;
            ////    try
            ////    {
            ////        itemScript.NextGradePrice = int.Parse(constantInfo.Deserialized["grades"][hull.Grade + 1]["priceItem"]["price"].ToString());
            ////    }
            ////    catch { }

            ////    var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
            ////    itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
            ////    if (userGarageDBComponent.selectedEquipment.Hulls.Contains(hull))
            ////    {
            ////        itemScript.Equiped = true;
            ////    }
            ////    itemScript.gameObject.SetActive(true);
            ////    itemScript.UpdateData();
            ////}
            ////foreach (var colormap in userGarageDBComponent.garage.Colormaps)
            ////{
            ////    var constantInfo = ConstantService.instance.ConstantDB[colormap.PathName];
            ////    //var resourcesData = ClientInit.ItemResourcesDBOld.ItemsResourcesDB[colormap.PathName].Values.ToList()[0];
            ////    var resourcesData = new ItemCard();
            ////    resourcesData += ResourcesService.instance.GameAssets.GetDirectory(colormap.PathName).FillChildContentToItem(colormap.PathName + "\\");
            ////    var newElement = Instantiate(PlayerItemsListElementTemplate, PlayerItemsList.transform);
            ////    var itemScript = newElement.GetComponent<GarageListItemElement>();
            ////    PlayerItemsListRepresentation.Add(colormap.PathName, itemScript);
            ////    itemScript.ItemPath = colormap.PathName;
            ////    itemScript.ItemGradeString = "";
            ////    itemScript.resourcesObj = resourcesData;
            ////    itemScript.constantObj = constantInfo;
            ////    //itemScript.Grade = hull.Grade;
            ////    itemScript.ItemCount = -1;
            ////    itemScript.UserItem = true;
            ////    //itemScript.NextGradePrice = int.Parse(constantInfo.Deserialized["grades"][hull.Grade + 1]["priceItem"]["price"].ToString());
            ////    var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
            ////    itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
            ////    if (userGarageDBComponent.selectedEquipment.Colormaps.Contains(colormap))
            ////    {
            ////        itemScript.Equiped = true;
            ////    }
            ////    itemScript.gameObject.SetActive(true);
            ////    itemScript.UpdateData();
            ////}

            #endregion
            #region ItemFilling

            ReprezentationCategoryPrepare();

            var itemsCount = GarageItemsDB.Count;

            foreach (var supplys in GlobalGameDataConfig.GarageShop.garageShop.Supplies)
            {
                //if (userGarageDBComponent.garage.Supplies.Where(x => x.PathName == supply.PathName).Count() > 0)
                //    continue;

                var supply = supplys;
                var constantInfo = ConstantService.instance.ConstantDB[supply.PathName];
                var userSupply = userGarageDBComponent.garage.Supplies.Where(x => x.PathName == supply.PathName && x.Count > 0);
                GarageListItemElement itemScript = null;
                if (!GarageItemsDB.ContainsKey(supply.PathName))
                {
                    var newElement = Instantiate(GarageItemsListElementTemplate, GarageItemsList.transform);
                    itemScript = newElement.GetComponent<GarageListItemElement>();
                    GarageItemsDB.Add(supply.PathName, itemScript);
                    GarageItemsListRepresentation[constantInfo.LibTree.HeadLib.Path].Add(supply.PathName, itemScript);
                }
                else
                {
                    itemScript = GarageItemsDB[supply.PathName];
                }


                itemScript.ItemPath = supply.PathName;
                itemScript.constantObj = constantInfo;
                itemScript.ItemCount = userSupply.Count() > 0 ? userSupply.Last().Count : 0;
                itemScript.Price = int.Parse(constantInfo.Deserialized["priceItem"]["price"].ToString());
                itemScript.Preview = ResourcesService.instance.GameAssets.GetDirectory(supply.PathName + "\\ui\\res").GetChildFSObject("preview").GetContent<Sprite>();

                var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
                itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
                itemScript.gameObject.SetActive(false);
                itemScript.UpdateData();
            }
            foreach (var supply in GlobalGameDataConfig.GarageShop.garageShop.ModuleWithTimeExpiration)
            {
                if (!GarageItemsDB.ContainsKey(supply.PathName))
                {

                }
            }

            var tankParts = GlobalGameDataConfig.GarageShop.garageShop.Turrets.Cast<TankPart>().Concat(GlobalGameDataConfig.GarageShop.garageShop.Hulls);
            var userTankParts = userGarageDBComponent.garage.Turrets.Cast<TankPart>().Concat(userGarageDBComponent.garage.Hulls);
            var userEquipedTankParts = userGarageDBComponent.selectedEquipment.Turrets.Cast<TankPart>().Concat(userGarageDBComponent.selectedEquipment.Hulls);
            var maxGrades = new Dictionary<string, int>();
            tankParts.ForEach(x => maxGrades[x.PathName] = tankParts.Max((turret) => {
                if (turret.PathName != x.PathName)
                    return -1;
                else
                    return turret.Grade;
            }));
            foreach (var cturret in tankParts)
            {
                var turret = cturret;
                var constantInfo = ConstantService.instance.ConstantDB[turret.PathName];
                var userTurret = userTankParts.Where(x => x.PathName == turret.PathName);
                if (userTurret.Count() > 0)
                {
                    turret = userTurret.Last();
                }
                else
                {
                    var turrets = tankParts.Where(x => x.PathName == turret.PathName);
                    turret = turrets.Where(x => x.Grade == turrets.Min((turret) => turret.Grade)).Last();
                    if (turret != cturret)
                        continue;
                }

                GarageListItemElement itemScript = null;
                if (!GarageItemsDB.ContainsKey(turret.PathName))
                {
                    var newElement = Instantiate(GarageItemsListElementTemplate, GarageItemsList.transform);
                    itemScript = newElement.GetComponent<GarageListItemElement>();
                    GarageItemsDB.Add(turret.PathName, itemScript);
                    GarageItemsListRepresentation[constantInfo.LibTree.HeadLib.Path].Add(turret.PathName, itemScript);
                }
                else
                {
                    itemScript = GarageItemsDB[turret.PathName];
                }
                var resourcesData = ItemCard.CreateInstance<ItemCard>();
                turret.Skins.ForEach(x => {
                    if (!ConstantService.instance.GetByConfigPath(x.SkinPathName).Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
                    {
                        resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName + "\\" + turret.Grade.ToString()).FillChildContentToItem(x.SkinPathName + "\\");
                    }
                    else
                    {
                        resourcesData += ResourcesService.instance.GameAssets.GetDirectory(x.SkinPathName).FillChildContentToItem(x.SkinPathName + "\\");
                    }
                });
                
                itemScript.ItemPath = turret.PathName;
                itemScript.SkinItemPath = turret.Skins;
                itemScript.ItemGradeString = "M" + turret.Grade.ToString();
                itemScript.resourcesObj = resourcesData;
                itemScript.constantObj = constantInfo;
                itemScript.MaxGrade = maxGrades[turret.PathName];
                itemScript.Grade = turret.Grade;
                itemScript.ItemCount = -1;
                try
                {
                    itemScript.Price = int.Parse(constantInfo.Deserialized["grades"][turret.Grade]["priceItem"]["price"].ToString());
                    if (userTurret.Count() > 0)
                    {
                        itemScript.UserGarage = true;
                        itemScript.NextGradePrice = int.Parse(constantInfo.Deserialized["grades"][turret.Grade + 1]["priceItem"]["price"].ToString());
                    }
                    else
                    {
                        itemScript.UserGarage = false;
                    }
                }
                catch {
                    //itemScript.NextGradePrice = -2;
                }
                var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
                itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
                if (userEquipedTankParts.Where(x => x.PathName == turret.PathName).Count() > 0)
                {
                    itemScript.Equiped = true;
                }
                else
                    itemScript.Equiped = false;
                itemScript.gameObject.SetActive(false);
                itemScript.UpdateData();

            }

            foreach (var colormapc in GlobalGameDataConfig.GarageShop.garageShop.Colormaps)
            {
                var colormap = colormapc;
                var constantInfo = ConstantService.instance.ConstantDB[colormap.PathName];
                var userTurret = userGarageDBComponent.garage.Colormaps.Where(x => x.PathName == colormap.PathName);
                GarageListItemElement itemScript = null;
                if (!GarageItemsDB.ContainsKey(colormap.PathName))
                {
                    var newElement = Instantiate(GarageItemsListElementTemplate, GarageItemsList.transform);
                    itemScript = newElement.GetComponent<GarageListItemElement>();
                    GarageItemsDB.Add(colormap.PathName, itemScript);
                    GarageItemsListRepresentation[constantInfo.LibTree.HeadLib.Path].Add(colormap.PathName, itemScript);
                }
                else
                {
                    itemScript = GarageItemsDB[colormap.PathName];
                }
                var resourcesData = ItemCard.CreateInstance<ItemCard>();
                try
                {
                    resourcesData += ResourcesService.instance.GameAssets.GetDirectory(colormap.PathName).FillChildContentToItem(colormap.PathName + "\\");
                }
                catch(Exception ex)
                {
                    ULogger.Error(colormap.PathName + " error load");
                }
                
                itemScript.ItemPath = colormap.PathName;
                itemScript.resourcesObj = resourcesData;
                itemScript.constantObj = constantInfo;
                itemScript.ItemCount = -1;
                try
                {
                    itemScript.Price = constantInfo.GetObject<int>("priceItem\\price");
                    if (userTurret.Count() > 0)
                    {
                        itemScript.UserGarage = true;
                    }
                    else
                    {
                        itemScript.UserGarage = false;
                    }
                }
                catch
                {
                }
                var itemName = constantInfo.Path.Substring(constantInfo.Path.IndexOf(constantInfo.LibName));
                itemScript.ItemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1);
                if (userGarageDBComponent.selectedEquipment.Colormaps.Where(x => x.PathName == colormap.PathName).Count() > 0)
                {
                    itemScript.Equiped = true;
                }
                else
                    itemScript.Equiped = false;
                itemScript.gameObject.SetActive(false);
                itemScript.UpdateData();
            }

            if (itemHandler.selectedItem != null)
            {
                itemHandler.OnSelect(itemHandler.selectedItem);
            }
            //if(itemsCount != GarageItemsDB.Count)
            NowCategory = NowCategory;
            #endregion

        }
    }
}