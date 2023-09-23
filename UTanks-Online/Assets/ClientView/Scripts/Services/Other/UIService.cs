using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.ClientControl.Services
{
    public class UIService : IService
    {
        public static UIService instance => IService.Get<UIService>();

        public GameObject UIHeader;

        public GameObject BackgroundUI { get; set; }
        public GameObject LoginRegisterUI { get; set; }
        public GameObject BattlesMainLobbyUI { get; set; }
        public GameObject BattleSelectorUI { get; set; }
        public GameObject BattleCreatorUI { get; set; }
        public GameObject LoadingWindowUI { get; set; }
        public GameObject BattleEnterUI { get; set; }
        public GameObject ChatUI { get; set; }
        public GameObject GarageUI { get; set; }
        private GarageUIHandler cacheGarageUIHandler = null;
        public GarageUIHandler garageUIHandler
        {
            get
            {
                if(cacheGarageUIHandler == null)
                {
                    cacheGarageUIHandler = GarageUI.GetComponent<GarageUIHandler>();
                }
                return cacheGarageUIHandler;
            }
        }
        
        public GameObject BattleUI { get; set; }
        private BattleUIHandler cacheBattleUIHandler = null;
        public BattleUIHandler battleUIHandler
        {
            get
            {
                if (cacheBattleUIHandler == null)
                {
                    cacheBattleUIHandler = BattleUI.GetComponent<BattleUIHandler>();
                }
                return cacheBattleUIHandler;
            }
        }
        public GameObject PlayerPanelUI { get; set; }
        public GameObject GameSettingsUI { get; set; }
        public SettingsUIHandler settingsUIHandler
        {
            get
            {
                if (GameSettingsUI != null)
                    return GameSettingsUI.GetComponent<SettingsUIHandler>();
                return null;
            }
        }
        void Start()
        {
            DontDestroyOnLoad(this);
            UIHeader = new GameObject() { name = "UIHeader" };

            BackgroundUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.BackgroundUI), UIHeader.transform);
            PlayerPanelUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.PlayerPanelUI), UIHeader.transform);
            LoginRegisterUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.LoginUI), UIHeader.transform);
            BattlesMainLobbyUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.BattleLobbyUI), UIHeader.transform);
            LoadingWindowUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.LoaderUI), UIHeader.transform);
            GameSettingsUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.SettingsUI), UIHeader.transform);
            BattleSelectorUI = BattlesMainLobbyUI.GetComponent<UICanvasGroup>().CanvasGroup["BattleSelectorUI"];
            BattleCreatorUI = BattlesMainLobbyUI.GetComponent<UICanvasGroup>().CanvasGroup["BattleCreatorUI"];
            BattleEnterUI = BattlesMainLobbyUI.GetComponent<UICanvasGroup>().CanvasGroup["BattleLobbyUI"];
            ChatUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.LobbyChatUI), UIHeader.transform);
            GarageUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.GarageUI), UIHeader.transform);
            BattleUI = Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.BattleUI), UIHeader.transform);
            Instantiate(ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.EventSystemUI), UIHeader.transform);
            HideAll();
            Show(new GameObject[] { BackgroundUI, LoadingWindowUI });
            settingsUIHandler.UpdateSettings();
            //BackgroundUI = Instantiate(BackgroundUI);
        }

        public void ShowAndHideOther(GameObject[] show, GameObject[] hide)
        {
            Array.ForEach(show, x => x.SetActive(true));
            Array.ForEach(hide, x => x.SetActive(false));
        }

        public void Show(GameObject[] show)
        {
            Array.ForEach(show, x => x.SetActive(true));
        }

        public void HideAll()
        {
            PlayerPanelUI.SetActive(false);
            BackgroundUI.SetActive(false);
            LoginRegisterUI.SetActive(false);
            //BattlesMainLobbyUI.SetActive(false);
            BattleSelectorUI.SetActive(false);
            LoadingWindowUI.SetActive(false);
            BattleCreatorUI.SetActive(false);
            BattleEnterUI.SetActive(false);
            ChatUI.SetActive(false);
            GarageUI.SetActive(false);
            BattleUI.SetActive(false);
            GameSettingsUI.SetActive(false);
        }

        public void ShowAll()
        {
            PlayerPanelUI.SetActive(true);
            BackgroundUI.SetActive(true);
            LoginRegisterUI.SetActive(true);
            BattlesMainLobbyUI.SetActive(true);
            LoadingWindowUI.SetActive(true);
            BattleSelectorUI.SetActive(true);
            BattleCreatorUI.SetActive(true);
            BattleEnterUI.SetActive(true);
            ChatUI.SetActive(true);
            GarageUI.SetActive(true);
            BattleUI.SetActive(true);
            GameSettingsUI.SetActive(true);
        }

        public void Hide(GameObject[] hide)
        {
            Array.ForEach(hide, x => x.SetActive(false));
        }

        public override void PostInitializeProcess()
        {
            
        }

        public override void InitializeProcess()
        {
            
        }

        public override void OnDestroyReaction()
        {
            
        }
    }
}