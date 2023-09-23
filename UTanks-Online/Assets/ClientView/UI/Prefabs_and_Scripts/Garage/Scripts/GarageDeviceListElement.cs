using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class GarageDeviceListElement : MonoBehaviour
    {
        public int Grade;
        public string DevicePath;
        public string ItemPath;
        public float DevicePrice;
        public ItemCard itemResources;
        public Image Preview;
        public Text DeviceName;
        public TMP_Text DeviceDescription;
        public Button SkinOperationButton;
        public GameObject PriceBlock;
        public TMP_Text PriceText;

        public void SendSkin()
        {
            UIService.instance.garageUIHandler.SelectDevice(DevicePath);
        }

        public void Initialize()
        {
            var skinConfig = ConstantService.instance.GetByConfigPath(DevicePath);
            Preview.sprite = UnityExtend.SpriteExtensions.TextureToSprite(itemResources.GetElement<Texture2D>(DevicePath + "\\preview"));
            PriceBlock.SetActive(false);
            //if (skinConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            //{
            //    Preview.sprite = UnityExtend.SpriteExtensions.TextureToSprite(ClientInit.ItemResourcesDBOld.ItemsResourcesDB[ItemPath][SkinPath].Preview[skinConfig.Deserialized["previewPathName"]["preview"].ToString()]);
            //}
            //else
            //{
            //    Preview.sprite = UnityExtend.SpriteExtensions.TextureToSprite(ClientInit.ItemResourcesDBOld.ItemsResourcesDB[ItemPath][Grade.ToString()].Preview[skinConfig.Deserialized["previewPathName"]["preview"].ToString()]);
            //}
            DeviceName.text = skinConfig.LibName;
            DeviceDescription.text = "";
        }
    }
}