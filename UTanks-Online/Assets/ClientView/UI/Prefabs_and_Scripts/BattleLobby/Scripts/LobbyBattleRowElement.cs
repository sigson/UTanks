using Assets.ClientCore.CoreImpl.ECS.Types.Battle;
using SecuredSpace.ClientControl.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Components.Battle;

namespace SecuredSpace.UI.GameUI
{
    public class LobbyBattleRowElement : MonoBehaviour
    {
        public Text MapRealNameText;
        public Text MapCustomNameText;
        public Text MapGoalValue;
        public GameObject PlayersCanvas;
        public LobbyBattleRowPlayerCounter PlayerCounterExample;
        public Dictionary<long, GameObject> PlayerCounterElements = new Dictionary<long, GameObject>();
        public Dictionary<long, LobbyInBattlePlayerCommandTable> BattlePlayersTablePlayerColumnsRepresentation = new Dictionary<long, LobbyInBattlePlayerCommandTable>();
        public Dictionary<long, EnterToBattleButton> EnterToBattleCommandButtonsRepresentation = new Dictionary<long, EnterToBattleButton>();
        public List<BattleEnterSettingsElement> battleEnterSettingsElements = new List<BattleEnterSettingsElement>();
        public BattleSimpleInfo battleSimpleInfo;
        public BattleComponent battleComponent;
        public bool Selected;
        public long EntityId;
        public Sprite Preview;

        public void OnSelectBattle()
        {
            var LobbyUIHandler = UIService.instance.BattlesMainLobbyUI.GetComponent<BattleLobbyUIHandler>();
            LobbyUIHandler.BattlesListRepresentation.Values.ForEach((battleElement) => battleElement.Selected = false);
            this.Selected = true;
            UIService.instance.BattleCreatorUI.SetActive(false);
            UIService.instance.BattleEnterUI.SetActive(true);
            LobbyUIHandler.BattlePreviewImage.GetComponent<Image>().sprite = Preview;
            LobbyUIHandler.ActivatedObjects.ForEach(x => x.SetActive(false));
            LobbyUIHandler.ActivatedObjects.Clear();
            LobbyUIHandler.BattlePreviewName.text = MapCustomNameText.GetComponent<Text>().text;
            BattlePlayersTablePlayerColumnsRepresentation.Values.ForEach(x => x.gameObject.SetActive(true));
            LobbyUIHandler.battleSettingsHandler.GoalValue.text = battleComponent.BattleWinGoalValue.ToString();
            LobbyUIHandler.battleSettingsHandler.TimeValue.text = battleComponent.BattleTimeMinutes.ToString();
            LobbyUIHandler.battleSettingsHandler.WeatherValue.text = "Weather: " + battleComponent.WeatherMode.ToString();
            LobbyUIHandler.battleSettingsHandler.MapDayTime.text = $"Map time: {battleComponent.TimeMode.ToString()}:00";
            EnterToBattleCommandButtonsRepresentation.Values.ForEach(x => x.gameObject.SetActive(true));
            battleEnterSettingsElements.ForEach(x => x.gameObject.SetActive(true));
            BattlePlayersTablePlayerColumnsRepresentation.Values.ForEach(x => LobbyUIHandler.ActivatedObjects.Add(x.gameObject));
            EnterToBattleCommandButtonsRepresentation.Values.ForEach(x => LobbyUIHandler.ActivatedObjects.Add(x.gameObject));
            battleEnterSettingsElements.ForEach(x => LobbyUIHandler.ActivatedObjects.Add(x.gameObject));
        }
    }
}