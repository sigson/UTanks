using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.Special;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.Extensions;
using UTanksClient.ClassExtensions;
using UTanksClient.Services;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.UnityExtend;
using System.Linq;

namespace SecuredSpace.UI.GameUI
{
    public class GarageItemHandler : MonoBehaviour
    {
        [SerializeField] private GameObject ItemWindowAllContent;
        [SerializeField] private GameObject ItemNameBlockElement;
        [SerializeField] private GameObject ItemCharacteristicsBlockElement;
        [SerializeField] private GameObject ItemEffectsBlockElement;
        [SerializeField] private GameObject ItemDevicesLabelBlockElement;
        [SerializeField] private GameObject ItemDevicesBlockElement;
        [SerializeField] private GameObject ItemDescriptionLabelBlockElement;
        [SerializeField] private GameObject ItemDescriptionText;
        [SerializeField] private GameObject CharacteristicHeaderEmptyFiller;
        [Space(10)]
        #region operationButton
        [Header("Buy")]

        [SerializeField] private GameObject BuyItemButtonObject;
        [SerializeField] private Button BuyItemButton;
        [SerializeField] private TMP_Text BuyItemTextOfOperationType;
        [SerializeField] private TMP_Text BuyPriceLabel;
        [SerializeField] private GameObject BuyRestrictRankPanel;
        [SerializeField] private GameObject BuyRestrictRankImagePanel;
        [SerializeField] private List<GameObject> BuyRestrictRankSeparator;
        [SerializeField] private TMP_Text BuyRestrictRankLabel;

        [SerializeField] private GameObject BuyItemCounterObject;
        [SerializeField] private InputField BuyItemCounter;
        public int ItemBuyCount => int.Parse(BuyItemCounter.text);
        [Space(10)]
        [Header("Setup")]
        [SerializeField] private GameObject SetupItemButtonObject;
        [SerializeField] private Button SetupItemButton;
        [SerializeField] private TMP_Text SetupItemTextOfOperationType;
        [Space(10)]
        [Header("Upgrade")]
        [SerializeField] private GameObject UpgradeItemButtonObject;
        [SerializeField] private Button UpgradeItemButton;
        [SerializeField] private TMP_Text UpgradeItemTextOfOperationType;
        #endregion
        [Space(10)]
        [Header("Devices")]
        [SerializeField] private Button SkinSlotButton;
        [SerializeField] private Image SkinSlotButtonOperationImage;
        [SerializeField] private Image SkinSlotPreview;
        [SerializeField] private GameObject LockSkinSlot;
        [SerializeField] private Button DeviceSlotButton;
        [SerializeField] private Image DeviceSlotButtonOperationImage;
        [SerializeField] private Image DeviceSlotPreview;
        [SerializeField] private GameObject LockDeviceSlot;
        [Space(10)]

        [SerializeField] private Image ItemInfoImgPreview;
        [SerializeField] private TMP_Text ItemInfoLabel;
        [SerializeField] private TMP_Text ItemNameLabel;
        [SerializeField] private TMP_Text ItemGradeLabel;

        [Space(20)]
        public List<GameObject> ActivatedCharacteristicObjects = new List<GameObject>();
        public CharacteristicHeaderElement CharacteristicHeaderElement;
        public Dictionary<string, CharacteristicHeaderElement> CharacteristicHeaders = new Dictionary<string, CharacteristicHeaderElement>();
        public CharacteristicTableChainElement CharacteristicTableChainElement;
        //public CharacteristicTableChainElementColumn CharacteristicTableChainElementColumn;
        public Dictionary<string, CharacteristicTableChainElement> CharacteristicTableRows = new Dictionary<string, CharacteristicTableChainElement>();
        public EffectTableElement EffectTableElementExample;
        public Dictionary<string, EffectTableElement> EffectsTable = new Dictionary<string, EffectTableElement>();
        [Space(10)]
        [SerializeField] private SerializableDictionary<DeviceSlotState, Sprite> DeviceSlotSpriteStates = new SerializableDictionary<DeviceSlotState, Sprite>();
        enum DeviceSlotState
        {
            Install,
            Remove
        }
        [Space(10)]
        public GarageListItemElement selectedItem;

        #region operations

        public void Buy(GarageListItemElement garageItem)
        {
            GarageBuyItemEvent weaponBuyEvent = garageItem.constantObj.HeadLibName != "supplies" && garageItem.constantObj.HeadLibName != "boosters" ? new GarageBuyItemEvent() { ConfigPath = garageItem.ItemPath, Grade = 0, Count = 0 } : new GarageBuyItemEvent() { ConfigPath = garageItem.ItemPath, Grade = 0, Count = ItemBuyCount };
            TaskEx.RunAsync(() => {
                ClientNetworkService.instance.Socket.emit<GameDataEvent>(weaponBuyEvent.PackToNetworkPacket());
            });
        }

        public void BuyUpgrade(GarageListItemElement garageItem)
        {
            var weaponBuyEvent = new GarageBuyItemEvent() { ConfigPath = garageItem.ItemPath, Grade = garageItem.Grade + 1, Count = 0 };
            TaskEx.RunAsync(() => {
                ClientNetworkService.instance.Socket.emit<GameDataEvent>(weaponBuyEvent.PackToNetworkPacket());
            });
        }

        public void Equip(GarageListItemElement garageItem)
        {
            var weaponChangeEvent = new WeaponChangeEvent() { ConfigPath = garageItem.ItemPath, Grade = garageItem.Grade, SkinConfigPath = garageItem.SkinItemPath.Count > 0 ? garageItem.SkinItemPath[0].SkinPathName : null };
            TaskEx.RunAsync(() => {
                ClientNetworkService.instance.Socket.emit<GameDataEvent>(weaponChangeEvent.PackToNetworkPacket());
            });

        }

        public void TryOn(GarageListItemElement garageItem)
        {

        }

        #endregion

        public void OnSelect(GarageListItemElement garageItem)
        {
            var garageUIHandler = UIService.instance.garageUIHandler;
            if(selectedItem != null)
                selectedItem.DeselectItem();
            selectedItem = garageItem;
            garageItem.Selected = true;

            #region setupRegion
            ItemWindowAllContent.SetActive(false);
            ItemNameBlockElement.SetActive(false);
            ItemCharacteristicsBlockElement.SetActive(false);
            ItemEffectsBlockElement.SetActive(false);
            ItemDevicesLabelBlockElement.SetActive(false);
            ItemDevicesBlockElement.SetActive(false);
            ItemDescriptionLabelBlockElement.SetActive(false);
            ItemDescriptionText.SetActive(false);


            BuyItemButtonObject.SetActive(false);
            SetupItemButtonObject.SetActive(false);
            UpgradeItemButtonObject.SetActive(false);
            BuyItemCounterObject.SetActive(false);

            BuyItemButton.onClick.RemoveAllListeners();
            SetupItemButton.onClick.RemoveAllListeners();
            UpgradeItemButton.onClick.RemoveAllListeners();
            SkinSlotButton.onClick.RemoveAllListeners();
            DeviceSlotButton.onClick.RemoveAllListeners();
            #endregion

            ItemWindowAllContent.SetActive(true);
            ItemNameBlockElement.SetActive(true);
            //ItemInfoImgPreview.sprite = garageItem.Preview;
            //ItemInfoImgPreview.gameObject.SetActive(true);
            ItemNameLabel.text = garageItem.ItemName;
            ItemGradeLabel.text = garageItem.ItemGradeString;
            ItemNameLabel.transform.parent.gameObject.SetActive(true);
            try
            {
                ItemInfoLabel.text = garageItem.constantObj.DeserializedInfo["descriptionItem"]["description"].ToString();
                ItemInfoLabel.gameObject.SetActive(true);
            }
            catch { }

            //ItemInfoLabel.transform.parent.gameObject.SetActive(true);
            if (!garageItem.UserGarage)
            {
                BuyItemButtonObject.SetActive(true);
                BuyItemButton.onClick.AddListener(() => Buy(UIService.instance.garageUIHandler.itemHandler.selectedItem));
                BuyItemTextOfOperationType.text = "Buy";
                if (garageItem.constantObj.HeadLibName == "colormap")
                {
                    SetupItemButton.onClick.AddListener(() => TryOn(UIService.instance.garageUIHandler.itemHandler.selectedItem));
                    SetupItemTextOfOperationType.text = "Try on";
                }
            }
            else
            {
                if (garageItem.ItemPath.Contains("garage\\hull\\") || garageItem.ItemPath.Contains("garage\\weapon\\"))
                {
                    #region prepare
                    ItemDevicesLabelBlockElement.SetActive(true);
                    ItemDevicesBlockElement.SetActive(true);
                    
                    SkinSlotButtonOperationImage.sprite = DeviceSlotSpriteStates[DeviceSlotState.Install];
                    SkinSlotButton.onClick.RemoveAllListeners();
                    SkinSlotButton.onClick.AddListener(() => UIService.instance.garageUIHandler.ShowDeviceWindow());
                    #endregion
                    if (garageItem.SkinItemPath.Count > 1)
                    {
                        var equipedSkin = garageItem.Equiped ? garageItem.SkinItemPath.Where(x => x.Equiped).Last() : garageItem.SkinItemPath.Where(x => x.SkinPathName.Contains("default")).Last();
                        if (!equipedSkin.SkinPathName.Contains("default") && equipedSkin != null)
                        {
                            SkinSlotPreview.enabled = true;
                            //SkinSlotButtonOperationImage.sprite = DeviceSlotSpriteStates[DeviceSlotState.Remove];
                            SkinSlotPreview.sprite = garageItem.resourcesObj.GetElement<Sprite>(equipedSkin.SkinPathName + "\\preview");
                        }
                        else
                        {
                            SkinSlotPreview.enabled = false;
                        }
                    }
                }
                else
                {
                    SkinSlotPreview.enabled = false;
                    //#region prepare
                    //SkinSlotPreview.sprite = null;
                    //SkinSlotButtonOperationImage.sprite = DeviceSlotSpriteStates[DeviceSlotState.Install];
                    //SkinSlotButton.onClick.RemoveAllListeners();
                    //SkinSlotButton.transform.parent.parent.gameObject.SetActive(false);
                    //#endregion
                }
                if (garageItem.NextGradePrice != -1)
                {
                    BuyItemButtonObject.SetActive(true);
                    BuyItemButton.onClick.AddListener(() => BuyUpgrade(UIService.instance.garageUIHandler.itemHandler.selectedItem));
                    BuyItemTextOfOperationType.text = "Upgrade";
                }
                else
                {
                    BuyItemButtonObject.SetActive(false);
                }
                SetupItemButtonObject.SetActive(true);
                SetupItemButton.onClick.AddListener(() => Equip(UIService.instance.garageUIHandler.itemHandler.selectedItem));
                SetupItemTextOfOperationType.text = "Equip";

            }
            if(garageItem.Price != -1 || (garageItem.UserGarage && garageItem.NextGradePrice != -1))
            {
                if (garageItem.Price != -1)
                {
                    BuyPriceLabel.text = garageItem.Price.ToString();
                }
                if (garageItem.UserGarage && garageItem.NextGradePrice != -1)
                {
                    BuyPriceLabel.text = garageItem.NextGradePrice.ToString();
                }
            }
            else
            {
                BuyPriceLabel.text = "";
            }
            if(garageItem.ItemCount != -1)
            {
                BuyItemCounterObject.SetActive(true);
            }
            else
                BuyItemCounterObject.SetActive(false);
            //OperationItemButtonPriceLabel.transform.parent.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();

            //UIService.instance.garageUIHandler.GarageItemsListRepresentation.Values.ForEach((garage) => garage.Values.ForEach(y => y.Selected = false));

            //selectedItem = garageItem;
            //garageItem.Selected = true;

            GenerateCharacterTable(garageItem);
        }

        public void ShowCharacteristics(GarageListItemElement garageItem)//deprecated
        {
            if (garageItem.ItemPath.Contains("garage\\hull\\") || garageItem.ItemPath.Contains("garage\\weapon\\"))
            {
                UnityAction<string, string> showProperty = (name, value) =>
                {
                    //UIService.instance.garageUIHandler.
                };
                foreach (var property in garageItem.constantObj.Deserialized["grades"][garageItem.Grade]["visualProperties"]["mainProperties"])
                {
                    var propertyName = property["name"].ToString();
                    var propertyValue = property["normalizedValue"].ToString();
                    showProperty(propertyName, propertyValue);
                }
                foreach (var property in garageItem.constantObj.Deserialized["grades"][garageItem.Grade]["visualProperties"]["properties"])
                {
                    var propertyName = property["name"].ToString();
                    var propertyValue = property["initialValue"].ToString();
                    showProperty(propertyName, propertyValue);
                }
            }
            else
            {
                //garageUIHandler.OpenSkinWindowButton.SetActive(false);
            }

        }

        public void GenerateCharacterTable(GarageListItemElement garageItem)
        {
            //CharacteristicHeaderElement.transform.parent.parent.gameObject.SetActive(false);
            //EffectTableElementExample.transform.parent.gameObject.SetActive(false);
            ActivatedCharacteristicObjects.ForEach(x => x.SetActive(false));
            ActivatedCharacteristicObjects.Clear();
            if (garageItem.ItemPath.Contains("garage\\hull\\") || garageItem.ItemPath.Contains("garage\\weapon\\"))
            {
                ItemCharacteristicsBlockElement.SetActive(true);
                if (!CharacteristicTableRows.ContainsKey(garageItem.constantObj.Path + "0"))
                {
                    SortedDictionary<string, Dictionary<string, string>> gradesProperty = new SortedDictionary<string, Dictionary<string, string>>();
                    HashSet<string> headers = new HashSet<string>();
                    var rankConfig = ConstantService.instance.GetByConfigPath("ranksconfig").Deserialized["ranksExperiencesConfig"]["ranksExperiences"];
                    foreach (var configGrade in garageItem.constantObj.Deserialized["grades"])
                    {
                        var grade = configGrade["grade"].ToString();
                        Dictionary<string, string> propertyValues = new Dictionary<string, string>();
                        foreach (var mainProperty in configGrade["visualProperties"]["mainProperties"])
                        {
                            string unit = "";
                            try
                            {
                                //unit = " " + mainProperty["unit"].ToString();
                            }
                            catch { }
                            propertyValues[mainProperty["name"].ToString()] = mainProperty["normalizedValue"].ToString() + unit;
                            headers.Add(mainProperty["name"].ToString());
                        }
                        foreach (var property in configGrade["visualProperties"]["properties"])
                        {
                            string unit = "";
                            try
                            {
                                //unit = " " + property["unit"].ToString();
                            }
                            catch { }
                            propertyValues[property["name"].ToString()] = property["initialValue"].ToString() + unit;
                            headers.Add(property["name"].ToString());
                        }
                        gradesProperty[grade] = propertyValues;
                    }
                    List<string> removedHeaders = new List<string>();
                    headers.ForEach(x =>
                    {
                        try
                        {
                            if (!CharacteristicHeaders.ContainsKey(x))
                            {
                                var headerSprite = ResourcesService.instance.GameAssets.GetDirectory(@"garage\ui\itemcharacter").GetChildFSObject("card").GetContent<ItemCard>().GetElement<Sprite>(x);
                                var newHeader = Instantiate(CharacteristicHeaderElement.gameObject, CharacteristicHeaderElement.transform.parent).GetComponent<CharacteristicHeaderElement>();
                                newHeader.Icon.sprite = headerSprite;
                                newHeader.CharacteristicName.text = x;
                                CharacteristicHeaders[x] = newHeader;
                            }
                            garageItem.characteristicHeaderElements.Add(CharacteristicHeaders[x]);
                        }
                        catch
                        {
                            removedHeaders.Add(x);
                        }
                    });
                    removedHeaders.ForEach(x => headers.Remove(x));
                    foreach (var gradePropertys in gradesProperty)
                    {
                        var newCharacterRow = Instantiate(CharacteristicTableChainElement, CharacteristicTableChainElement.transform.parent).GetComponent<CharacteristicTableChainElement>();
                        foreach (var header in headers)
                        {
                            var newCharacterRowColumn = Instantiate(newCharacterRow.characteristicTableChainElementColumnExample, newCharacterRow.characteristicTableChainElementColumnExample.transform.parent).GetComponent<CharacteristicTableChainElementColumn>();
                            newCharacterRowColumn.CharacteristicValue.text = gradePropertys.Value[header];
                            newCharacterRow.characteristicColumns.Add(header, newCharacterRowColumn);
                            newCharacterRowColumn.CharacteristicValue.gameObject.SetActive(true);
                            newCharacterRowColumn.gameObject.SetActive(true);
                        }
                        //newCharacterRow.gameObject.SetActive(true);
                        var selectedGrade = garageItem.constantObj.Deserialized["grades"][int.Parse(gradePropertys.Key)];
                        newCharacterRow.GradePrice.text = selectedGrade["priceItem"]["price"].ToString();
                        newCharacterRow.GradeContainer.GetComponent<Text>().text = "M" + gradePropertys.Key;
                        var rankName = rankConfig[int.Parse(selectedGrade["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString())]["name"].ToString();
                        newCharacterRow.RankAccessIcon.sprite = RankService.instance.GetRank(rankName).miniSprite;
                        //newCharacterRow.RankAccessIcon.transform.SetAsLastSibling();
                        //////newCharacterRow.GetComponent<ContentLayoutRefreshMarker>().DirectUpdate();
                        CharacteristicTableRows[garageItem.constantObj.Path + gradePropertys.Key] = newCharacterRow;
                    }
                }


                int gradeCounter = 0;
                while (CharacteristicTableRows.ContainsKey(garageItem.constantObj.Path + gradeCounter.ToString()))
                {
                    var rowActive = CharacteristicTableRows[garageItem.constantObj.Path + gradeCounter.ToString()];
                    rowActive.transform.SetAsLastSibling();
                    rowActive.gameObject.SetActive(true);
                    rowActive.characteristicColumns.ForEach(x =>
                    {
                        var headerObj = CharacteristicHeaders[x.Key];
                        if (!headerObj.gameObject.activeInHierarchy)
                        {
                            headerObj.gameObject.SetActive(true);
                            ActivatedCharacteristicObjects.Add(CharacteristicHeaders[x.Key].gameObject);
                        }
                    });
                    ActivatedCharacteristicObjects.Add(rowActive.gameObject);
                    gradeCounter++;
                }
                //CharacteristicHeaderElement.transform.parent.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
                //CharacteristicTableChainElement.transform.parent.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
            }
            else if (garageItem.ItemPath.Contains("garage\\weaponkit\\"))
            {
                //CharacteristicHeaderElement.transform.parent.parent.gameObject.SetActive(true);
            }
            else
            {
                ItemEffectsBlockElement.gameObject.SetActive(true);
                if (garageItem.ItemPath.Contains("garage\\colormap\\"))
                {
                    foreach (var resist in garageItem.constantObj.Deserialized["weaponResists"])
                    {
                        var strResist = resist["weaponResist"].ToString();
                        if (strResist == @"garage\weapon\nullresist")
                            continue;
                        if (!EffectsTable.ContainsKey(strResist))
                        {
                            var newResist = Instantiate(EffectTableElementExample, EffectTableElementExample.transform.parent).GetComponent<EffectTableElement>();
                            newResist.Icon.sprite = ResourcesService.instance.GameAssets.GetDirectory(@"garage\ui\itemresistance").GetChildFSObject("card").GetContent<ItemCard>().GetElement<Sprite>(strResist);
                            EffectsTable[strResist] = newResist;
                        }
                        var resistObj = EffectsTable[strResist];
                        resistObj.gameObject.SetActive(true);
                        resistObj.Value.text = resist["resistPercents"].ToString();
                        ActivatedCharacteristicObjects.Add(resistObj.gameObject);
                    }
                }
            }
            CharacteristicHeaderEmptyFiller.transform.SetAsLastSibling();
        }

    }
}