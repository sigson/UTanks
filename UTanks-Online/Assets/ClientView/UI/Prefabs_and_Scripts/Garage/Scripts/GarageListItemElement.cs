using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.UI.Special;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.ECS.Types;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class GarageListItemElement : MonoBehaviour
    {
        [NonSerialized]public Sprite Preview;
        public int Price = -1;
        public int NextGradePrice = -1;
        public string ItemPath;
        public List<Skin> SkinItemPath = new List<Skin>();
        public List<Module> ModuleItemPath = new List<Module>();
        public string ItemName;
        private int _grade = -1;
        public int MaxGrade = 0;
        public int Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = value;
                if (MaxGrade == 0)
                    return;
                for (int i = 1; i < 4; i++)
                {
                    if (gradeVisualView.Count != 3)
                    {
                        var gradeVisInst = Instantiate(garageListItemGradeVisualPattern.gameObject, garageListItemGradeVisualPattern.transform.parent).GetComponent<GarageListItemGradeVisual>();
                        gradeVisualView.Add(gradeVisInst);
                    }
                    var gradeVis = gradeVisualView[i-1];
                    gradeVis.gameObject.SetActive(true);
                    gradeVis.GradeFilling.gameObject.SetActive(false);
                    if (_grade >= i)
                        gradeVis.GradeFilling.gameObject.SetActive(true);
                }
            }
        }
        public string ItemGradeString = "";
        public int ItemCount = -1;
        public long ItemExpirationTime = -1;
        public int Sale = 0;
        public bool UserGarage = false;

        private bool _selected;
        public bool Selected {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }
        private bool _equiped = false;
        public bool Equiped
        {
            get
            {
                return _equiped;
            }
            set
            {
                EquipedBorder.SetActive(value);
                _equiped = value;
            }
        }

        #region local_res
        [Space(10)]
        [SerializeField]private GameObject SalePanel;
        [SerializeField] private Text SaleValue;
        [SerializeField] private Text SaleTimeRemain;
        [Space(10)]
        public ConfigObj constantObj = null;
        public ItemCard resourcesObj = null;
        [Space(10)]
        [SerializeField] private Text ItemPriceText;
        [SerializeField] private Image ItemPreview;
        [SerializeField] private Text ItemNameText;
        [SerializeField] private Image CrystalIcon;
        [SerializeField] private Text ItemCountText;
        [SerializeField] private Image RankAccess;
        [SerializeField] private Image ItemBackground;
        [SerializeField] private GarageListItemGradeVisual garageListItemGradeVisualPattern;

        [SerializeField] private GameObject ItemPricePanel;
        [SerializeField] private GameObject ItemGradePanel;
        [SerializeField] private GameObject EquipedBorder;
        public Dictionary<int, CharacteristicTableChainElement> CharacteristicRows = new Dictionary<int, CharacteristicTableChainElement>();
        public List<CharacteristicHeaderElement> characteristicHeaderElements = new List<CharacteristicHeaderElement>();
        [SerializeField] private List<GarageListItemGradeVisual> gradeVisualView = new List<GarageListItemGradeVisual>();
        public Dictionary<string, string> elementsEffect = new Dictionary<string, string>();
        #endregion

        private void UpdateItemBackground()
        {
            string suffix = Selected ? "Selected" : "";
            if (UserGarage)
            {
                ItemBackground.sprite = UIService.instance.garageUIHandler.GarageItemBackgroundStates["Buyed" + suffix];
            }
            else
            {
                ItemBackground.sprite = UIService.instance.garageUIHandler.GarageItemBackgroundStates["Accessible" + suffix];
                if (RankAccess.enabled)
                {
                    ItemBackground.sprite = UIService.instance.garageUIHandler.GarageItemBackgroundStates["Unaccessible" + suffix];
                }
            }
        }

        public void SelectItem()
        {
            UIService.instance.garageUIHandler.itemHandler.OnSelect(this);
            UpdateItemBackground();
        }

        public void DeselectItem()
        {
            Selected = false;
            UpdateItemBackground();
        }

        public void UpdateData()
        {
            #region prepare
            RankAccess.enabled = false;
            ItemPricePanel.SetActive(false);
            ItemGradePanel.SetActive(false);
            ItemCountText.gameObject.SetActive(false);
            #endregion
            if ((ItemPath != null || ItemPath != "") && constantObj != null)
            {
                var defaultSkinList = SkinItemPath.Where(x => x.SkinPathName.Contains("default")).ToList();
                var defaultSkin = (defaultSkinList.Count > 0 ? defaultSkinList[0].SkinPathName : ItemPath);
                if (resourcesObj != null && resourcesObj.ItemData.ContainsKey(defaultSkin + "\\preview"))
                {
                    var previewTexture = resourcesObj.GetElement<Texture2D>(defaultSkin + "\\preview");
                    ItemPreview.GetComponent<Image>().sprite = Sprite.Create(previewTexture, new Rect(0, 0, previewTexture.width, previewTexture.height), new Vector2());
                }
                else
                {
                    ItemPreview.GetComponent<Image>().sprite = Preview;
                }
                Preview = ItemPreview.sprite;
                ItemNameText.GetComponent<Text>().text = ItemName + " " + ItemGradeString;
                CrystalIcon.gameObject.SetActive(true);
                if (Price != -1 && !UserGarage)
                {
                    ItemPriceText.GetComponent<Text>().text = Price.ToString();
                    ItemPricePanel.SetActive(true);
                    ItemGradePanel.SetActive(false);
                }
                if (Grade != -1 && UserGarage)
                {
                    //ItemPriceText.GetComponent<Text>().text = NextGradePrice.ToString();
                    ItemPricePanel.SetActive(false);
                    ItemGradePanel.SetActive(true);
                }
                
                //else
                //{
                //    ItemPriceText.GetComponent<Text>().text = "";
                //    CrystalIcon.gameObject.SetActive(false);
                //}
            }
            if (ItemCount != -1 && ItemCount != 0)
            {
                ItemCountText.gameObject.SetActive(true);
                ItemCountText.text = "x" + ItemCount.ToString();
            }
            int restrictRank = constantObj.Deserialized.ContainsKey("grades") ? constantObj.GetObject<int>($"grades\\{this.Grade}\\crystalsPurchaseUserRankRestriction\\restrictionValue") : constantObj.GetObject<int>("crystalsPurchaseUserRankRestriction\\restrictionValue");
            if (restrictRank > ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId].GetComponent<UserRankComponent>().Rank)
            {
                RankAccess.sprite = RankService.instance.GetRank(restrictRank).normalSprite;
                RankAccess.enabled = true;
            }
            else
            {
                RankAccess.enabled = false;
            }
            UpdateItemBackground();
        }
    
    }
}