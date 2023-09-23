using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using Assets.ClientCore.CoreImpl.ECS.Types.Battle;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.GameUI
{
    public class BattleLobbyUIHandler : MonoBehaviour
    {
        [Header("BattleSelectorUI")]
        public Button CreateBattleButton;
        public GameObject BattlesList;
        public LobbyBattleRowElement BattlesListRowElementPattern;
        public Dictionary<long, LobbyBattleRowElement> BattlesListRepresentation = new Dictionary<long, LobbyBattleRowElement>();
        public List<GameObject> ActivatedObjects = new List<GameObject>();
        [Space(10)]
        [Header("BattleCreatorUI")]
        public BattleCreatorUIHandler battleCreatorUIHandler;
        [Space(10)]
        [Header("BattleEnterUI")]
        public Image BattlePreviewImage;
        public BattleSettingsHandler battleSettingsHandler;
        public LobbyInBattlePlayerCommandTable BattlePlayersTablePlayerColumnExample;
        public BattleEnterSettingsElement BattleEnterSettingsElementExample;
        public TMP_Text BattlePreviewName;
        public EnterToBattleButton EnterToBattleExample;

        void Start()
        {
            ClientInitService.instance.OnLoadedGame += (ev, ev2) => StartImpl();
            //EnterToBattle.GetComponent<Button>().onClick.AddListener(FirstCommandEnterToBattle);
        }

        public void StartImpl()
        {
            BattlesListRowElementPattern.gameObject.SetActive(false);
            BattlePlayersTablePlayerColumnExample.gameObject.SetActive(false);
            CreateBattleButton.GetComponent<Button>().onClick.AddListener(OnCreateBattleButton);
            battleCreatorUIHandler.StartImpl();
        }

        public void OnCreateBattleButton()
        {
            UIService.instance.Hide(new GameObject[] { UIService.instance.BattleCreatorUI, UIService.instance.BattleEnterUI });
            UIService.instance.BattleCreatorUI.SetActive(true);
        }

        

        public void UpdateBattleList(BattleSimpleInfo battleSimpleInfo, ECSEntity battleEntity)
        {
            LobbyBattleRowElement battleElement;
            var battleComponent = battleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
            if (BattlesListRepresentation.TryGetValue(battleEntity.instanceId, out battleElement))
            {
                battleElement.MapCustomNameText.text = battleComponent.BattleCustomName;
                battleElement.MapRealNameText.text = battleComponent.BattleRealName;
                battleElement.battleSimpleInfo = battleSimpleInfo;
                if (!Lambda.TryExecute(() => battleElement.Preview = ResourcesService.instance.GameAssets.GetDirectory("maps\\ui\\preview\\res").GetChildFSObject(battleComponent.MapPath.Replace("Data\\Maps\\","").Replace(".xml", "")).GetContent<Sprite>()))
                    battleElement.Preview = null;
                battleElement.battleComponent = battleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
                battleElement.PlayerCounterExample.gameObject.SetActive(false);
                
                //var counter = Instantiate(battleElement.PlayerCounterExample, battleElement.PlayersBlock);
            }
            else
            {
                var newBattleRow = Instantiate(BattlesListRowElementPattern.gameObject, BattlesList.transform);
                var newBattleRowScript = newBattleRow.GetComponent<LobbyBattleRowElement>();
                newBattleRowScript.EntityId = battleEntity.instanceId;
                newBattleRowScript.battleSimpleInfo = battleSimpleInfo;
                newBattleRowScript.battleComponent = battleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
                if (!Lambda.TryExecute(() => newBattleRowScript.Preview = ResourcesService.instance.GameAssets.GetDirectory("maps\\ui\\preview\\res").GetChildFSObject(battleComponent.MapPath.Replace("Data\\Maps\\", "").Replace(".xml", "")).GetContent<Sprite>()))
                    newBattleRowScript.Preview = null;
                newBattleRowScript.MapCustomNameText.text = battleComponent.BattleCustomName;
                newBattleRowScript.PlayerCounterExample.gameObject.SetActive(false);
                newBattleRowScript.MapRealNameText.text = battleComponent.BattleRealName;

                //

                //var battleModeSettings = Instantiate(this.BattleEnterSettingsElementExample, this.BattleEnterSettingsElementExample.transform.parent).GetComponent<BattleEnterSettingsElement>();
                //battleModeSettings.Icon.sprite = ClientInitService.instance.ItemResourcesDBOld.UIObjectsDB["BattleUI"]["BattleMode"].Sprites[battleComponent.BattleMode.ToUpper() + "_MiniIcon"];
                //battleModeSettings.Icon.gameObject.SetActive(true);
                //battleModeSettings.Value.text = battleComponent.BattleWinGoalValue.ToString();
                //battleModeSettings.Value.gameObject.SetActive(true);
                //newBattleRowScript.battleEnterSettingsElements.Add(battleModeSettings);

                //var battleTimeSettings = Instantiate(this.BattleEnterSettingsElementExample, this.BattleEnterSettingsElementExample.transform.parent).GetComponent<BattleEnterSettingsElement>();
                //battleTimeSettings.Value.text = battleComponent.BattleTimeMinutes.ToString() + ":00";
                //battleTimeSettings.Value.gameObject.SetActive(true);
                //newBattleRowScript.battleEnterSettingsElements.Add(battleTimeSettings);

                //

                BattlesListRepresentation.Add(battleEntity.instanceId, newBattleRowScript);
                battleElement = newBattleRowScript;
                
                newBattleRow.SetActive(true);
            }
            foreach (var command in battleSimpleInfo.Commands)
            {
                var teamColor = ColorEx.ToColor(TeamColors.colors[command.Value.TeamColor]);
                GameObject commandCounter;
                if (battleElement.PlayerCounterElements.TryGetValue(command.Key, out commandCounter))
                {
                    commandCounter.GetComponent<LobbyBattleRowPlayerCounter>().CounterText.text = command.Value.commandPlayers.Count.ToString();
                }
                else
                {
                    commandCounter = Instantiate(battleElement.PlayerCounterExample.gameObject, battleElement.PlayersCanvas.transform);
                    commandCounter.GetComponent<LobbyBattleRowPlayerCounter>().CounterText.text = command.Value.commandPlayers.Count.ToString();
                    commandCounter.SetActive(true);
                    battleElement.PlayerCounterElements.Add(command.Key, commandCounter);
                }
                var counterOffset = TeamColors.elementColorOffsets["battlePlayerRowsBackground"];
                //Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2)
                commandCounter.GetComponent<LobbyBattleRowPlayerCounter>().PlayerCounterBackground.UpdateColor(teamColor);
                EnterToBattleButton commandButton;
                if(!battleElement.EnterToBattleCommandButtonsRepresentation.TryGetValue(command.Key, out commandButton))
                {
                    commandButton = Instantiate(EnterToBattleExample.gameObject, EnterToBattleExample.transform.parent).GetComponent<EnterToBattleButton>();
                    if (battleElement.EnterToBattleCommandButtonsRepresentation.Count == 0 && battleSimpleInfo.Commands.Count > 1)
                    {
                        commandButton.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>().reverseArrangement = false;
                    }
                    battleElement.EnterToBattleCommandButtonsRepresentation[command.Key] = commandButton;
                    commandButton.commandId = command.Key;
                    commandButton.FlagSprite.gameObject.SetActive(false);
                    if (!battleEntity.HasComponent<DMComponent>())
                    {
                        commandButton.FlagSprite.gameObject.SetActive(true);
                        commandButton.FlagSprite.UpdateColor(teamColor);
                    }
                }
                commandButton.TeamGoal.GetAdapter().text = command.Value.GoalScore.ToString();
                LobbyInBattlePlayerCommandTable commandTable;
                if (!battleElement.BattlePlayersTablePlayerColumnsRepresentation.TryGetValue(command.Key, out commandTable))
                {
                    commandTable = Instantiate(BattlePlayersTablePlayerColumnExample.gameObject, BattlePlayersTablePlayerColumnExample.transform.parent).GetComponent<LobbyInBattlePlayerCommandTable>();
                    battleElement.BattlePlayersTablePlayerColumnsRepresentation[command.Key] = commandTable;
                    commandTable.commandEntityId = command.Key;
                    commandTable.Color = command.Value.TeamColor;
                }

                //Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2)
                commandTable.GetComponent<Colorizer>().UpdateColor(teamColor);
                HashSet<long> activePlayers = new HashSet<long>();
                foreach (var comPlayer in command.Value.commandPlayers)
                {
                    LobbyInBattleCommandPlayerRow playerRow;
                    if (!commandTable.CommandPlayersListRepresentation.TryGetValue(comPlayer.Key, out playerRow))
                    {
                        playerRow = Instantiate(commandTable.InBattlePlayerRowElementPattern.gameObject, commandTable.InBattlePlayersCanvas.transform).GetComponent<LobbyInBattleCommandPlayerRow>();
                        commandTable.CommandPlayersListRepresentation[comPlayer.Key] = playerRow;
                        playerRow.playerEntityId = comPlayer.Key;
                        playerRow.gameObject.SetActive(true);
                        counterOffset = TeamColors.elementColorOffsets["battlePlayerRow"];
                        //playerRow.GetComponent<Image>().color = Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
                    }
                    playerRow.PlayerUsername.text = comPlayer.Value.Username;
                    playerRow.PlayerScore.text = comPlayer.Value.Score.ToString();
                    activePlayers.Add(comPlayer.Key);
                    //commandTable.InBattlePlayerRowElementPattern commandTable.InBattlePlayersCanvas.transform
                }
                List<long> RemovePlayers = new List<long>();
                foreach(var comPlayerRep in commandTable.CommandPlayersListRepresentation)
                {
                    if (!activePlayers.Contains(comPlayerRep.Key))
                        RemovePlayers.Add(comPlayerRep.Key);
                }
                RemovePlayers.ForEach(x =>
                {
                    Destroy(commandTable.CommandPlayersListRepresentation[x].gameObject);
                    commandTable.CommandPlayersListRepresentation.Remove(x);
                });
            }
        }

        public void FirstCommandEnterToBattle(EnterToBattleButton enterToBattleButton)
        {
            var selectedBattle = BattlesListRepresentation.Values.Where(x => x.Selected).ToList()[0];
            var enterToBattleEvent = new EnterToBattleEvent()
            {
                BattleId = selectedBattle.EntityId,
                TeamInstanceId = enterToBattleButton.commandId//selectedBattle.battleSimpleInfo.Commands.Keys.ToList()[0]//
            };
            TaskEx.RunAsync(() =>
            {
                ClientNetworkService.instance.Socket.emit(enterToBattleEvent.PackToNetworkPacket());
            });
        }
    }
}