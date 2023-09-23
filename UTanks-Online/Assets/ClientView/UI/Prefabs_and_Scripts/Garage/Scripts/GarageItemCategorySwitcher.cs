using SecuredSpace.ClientControl.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class GarageItemCategorySwitcher : MonoBehaviour
    {
        public string Category;
        public Button CategoryButton;
        public bool AwakeActive = false;

        private void Start()
        {
            if(AwakeActive && UIService.tryGetInstance<UIService>() != null)
                OnActivate();
        }

        public void OnActivate()
        {
            UIService.instance.garageUIHandler.Categories.Where(x => x.Value == this).ToList().ForEach(x => {
                UIService.instance.garageUIHandler.NowCategory = x.Key;
                UIService.instance.garageUIHandler.AdditionCategory = true;
            });
            UIService.instance.garageUIHandler.AdditionCategory = false;
        }
    }
}