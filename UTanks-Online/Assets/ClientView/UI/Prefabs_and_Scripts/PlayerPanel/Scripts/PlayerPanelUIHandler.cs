using SecuredSpace.ClientControl.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class PlayerPanelUIHandler : MonoBehaviour
    {
        public TMP_Text CrystalValue;
        public TMP_Text PlayerScoreValue;
        public Slider PlayerScoreProgressBar;
        public Image RankImage;
        [Space(10)]
        public Button GoToGarageButton;
        public Button GoToBattlesButton;
        public Button OpenShopWindowButton;
        public Button OpenRatingButton;
        public Button OpenMissionsWindowButton;
        public Button OpenFriendsWindowButton;
        public Button OpenSettingsWindowButton;
        public Button OpenSoundWindowButton;
        public Button OpenHelpWindowButton;
        public Button FullScreenButton;
        public Button ExitFromAccountButton;

        [Space(10)]

        private string Rank = "";
        private int Score = 0;
        private string Username = "";
        private int MinScore = 0;
        private int MaxScore = 0;

        // Start is called before the first frame update
        void Start()
        {
            GoToGarageButton.GetComponent<Button>().onClick.AddListener(OnGoToGarageButton);
            GoToBattlesButton.GetComponent<Button>().onClick.AddListener(OnGoToBattlesButton);
            OpenSettingsWindowButton.GetComponent<Button>().onClick.AddListener(OnOpenSettings);
            ExitFromAccountButton.GetComponent<Button>().onClick.AddListener(ExitFromAccount);
        }

        #region buttonsBlock
        private void OnGoToGarageButton()
        {
            var playerEntity = ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId];
            if (playerEntity.HasComponent(BattleOwnerComponent.Id))
            {
                if (UIService.instance.GarageUI.activeInHierarchy)
                {
                    UIService.instance.BattleUI.SetActive(true);
                    UIService.instance.GarageUI.SetActive(false);
                }
                else
                {
                    var battleEntity = ManagerScope.entityManager.EntityStorage[playerEntity.GetComponent<BattleOwnerComponent>(BattleOwnerComponent.Id).BattleInstanceId];
                    if (battleEntity.GetComponent<BattleComponent>().enableDressingUp)
                    {
                        UIService.instance.BattleUI.SetActive(false);
                        UIService.instance.GarageUI.SetActive(true);
                    }
                    else
                    {
                        MessageBoxProvider.ShowInfo("Exit from battle", "Inbattle dressing up disabled");
                    }
                }
            }
            else
            {
                UIService.instance.BattleCreatorUI.SetActive(false);
                UIService.instance.BattleSelectorUI.SetActive(false);
                UIService.instance.BattleEnterUI.SetActive(false);
                UIService.instance.GarageUI.SetActive(true);
            }

        }

        private void OnGoToBattlesButton()
        {
            var playerEntity = ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId];
            if (UIService.instance.BattleUI.activeInHierarchy || playerEntity.HasComponent(BattleOwnerComponent.Id))
            {
                //LeaveFromBattleEvent
                var battleLoadedEvent = new LeaveFromBattleEvent()
                {
                    BattleId = playerEntity.GetComponent<BattleOwnerComponent>().BattleInstanceId
                };
                TaskEx.RunAsync(() =>
                {
                    ClientNetworkService.instance.Socket.emit(battleLoadedEvent.PackToNetworkPacket());
                });
                //ClientInit.battleManager.ExitFromBattle();
                UIService.instance.BattleUI.SetActive(false);
                UIService.instance.BattleSelectorUI.SetActive(true);
                UIService.instance.ChatUI.SetActive(true);
                UIService.instance.BackgroundUI.SetActive(true);
                //ClientInit.uiManager.BattleEnterUI.SetActive(true);
                UIService.instance.GarageUI.SetActive(false);
            }
            else
            {
                //ClientInit.uiManager.BattleCreatorUI.SetActive(true);
                UIService.instance.BattleSelectorUI.SetActive(true);
                //ClientInit.uiManager.BattleEnterUI.SetActive(true);
                UIService.instance.GarageUI.SetActive(false);
            }



        }
        
        private void OnOpenSettings()
        {
            UIService.instance.GameSettingsUI.SetActive(true);
        }
        
        private void ExitFromAccount()
        {
            File.WriteAllText(Application.streamingAssetsPath + "/loginconfig.json",
                "{\"LoginData\":{\"login\":\"\",\"password\":\"\"}}"
                );
        }
        #endregion


        public void ChangeCrystalCount(int newCrystalCount)
        {
            CrystalValue.text = newCrystalCount.ToString();
        }

        public void ChangeScore(int newScore)
        {
            Score = newScore;
            UpdateUserBar();
        }

        public void SetupRank(int newRank)
        {
            MinScore = Convert.ToInt32(RankService.instance.GetRank(newRank).scoreThreshold);
            Rank = RankService.instance.GetRank(newRank).name;
            MaxScore = Convert.ToInt32(RankService.instance.GetRank(newRank + 1).scoreThreshold);
            RankImage.sprite = RankService.instance.GetRank(newRank).normalSprite;
            UpdateUserBar();
        }

        public void ChangeUsername(string username)
        {
            Username = username;
            UpdateUserBar();
        }

        private void UpdateUserBar()
        {
            if(MaxScore == -1)
            {
                PlayerScoreValue.text = Score.ToString() + " / " + "  " + Rank + " " + Username;
                PlayerScoreProgressBar.value = 1f;
            }
            else if(!(Score == 0 && MaxScore == 0))
            {
                PlayerScoreValue.text = Score.ToString() + " / " + MaxScore.ToString() + "  " + Rank + " " + Username;
                PlayerScoreProgressBar.value = ((float)Score - MinScore) / ((float)MaxScore - MinScore);
            }
        }
    }

}