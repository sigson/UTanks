using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using Assets.ClientCore.CoreImpl.ECS.Types.Battle;
using SecuredSpace.Battle.Tank;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.Special;
using SecuredSpace.UnityExtend;
using SecuredSpace.UnityExtend.Timer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Events.Chat;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.Extensions;
using UTanksClient.Services;
using SecuredSpace.Battle;
using UTanksClient.ECS.Components;
using SecuredSpace.ClientControl.DBResources;
using UTanksClient.ClassExtensions;
using TMPro;

namespace SecuredSpace.UI.GameUI
{
    public class BattleUIHandler : MonoBehaviour
    {
        public GameObject SupplyPanel;
        public BattleSupplyElement SupplyExample;
        public SerializableDictionary<string, BattleSupplyElement> Supplies;
        [Space(10)]
        public GameObject KilllogPanel;
        public GameObject KilllogContent;
        public KillLogRowElement KilllogRowExample;
        public List<GameObject> KillLogs = new List<GameObject>();
        [Space(10)]
        public GameObject ChatPanel;
        public GameObject ChatCanvas;
        public ChatMessageElement ChatMessageExample;
        public InputField MessageInput;
        public SerializableDictionary<ChatMessageElement, long> messages = new SerializableDictionary<ChatMessageElement, long>();
        [Space(10)]
        public GameObject BattleEventsLogPanel;
        public GameObject BattleEventsLogCanvas;
        public BattleEventLogElement BattleEventsLogExample;
        [Space(10)]
        public GameObject BattleInfoPanel;
        public TMP_Text BattleInfoFundValue;
        public GameObject BattleInfoFundIcon;
        
        public Image BattleInfoGoalIcon;
        public TMP_Text BattleInfoGoalValue;

        public float battleRemainingTime;
        public float battleRoundTime;
        public Image BattleInfoTimeIcon;
        public TMP_Text BattleInfoTimeText;
        private UnityTimer battleTimeTimer;
        [Space(10)]
        public GameObject TeamStatsPanel;
        public TeamGoalElement LeftTeamGoal;
        public TeamGoalElement RightTeamGoal;
        public Dictionary<long, TeamGoalElement> teamStats = new Dictionary<long, TeamGoalElement>();
        [Space(10)]
        public GameObject StatsWindow;
        public StatsCommandElement ExampleCommand;
        public Text StatsMapName;
        public SerializableDictionary<long, StatsCommandElement> statCommands = new SerializableDictionary<long, StatsCommandElement>();
        [Space(10)]
        public GameObject SelfDestructionWindow;

        public int EnterKeyDown = 0;
        public int EnterKeyUp = 0;
        public void Update()
        {
            if (!ClientInitService.instance.LockInput && Input.GetKeyDown(KeyCode.Tab))
            {
                StatsWindow.SetActive(true);
            }
            if (!ClientInitService.instance.LockInput && Input.GetKeyUp(KeyCode.Tab))
            {
                StatsWindow.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Return))
                EnterKeyDown = 1;
            if (Input.GetKeyUp(KeyCode.Return) && EnterKeyDown == 1)
                EnterKeyUp = 1;

            if (ClientInitService.instance.LockInput && EnterKeyDown == 1 && EnterKeyUp == 1)
            {
                ClientInitService.instance.LockInput = false;
                MessageInput.transform.parent.parent.gameObject.SetActive(false);
                EnterKeyDown = 0;
                EnterKeyUp = 0;
                SendChatMessage();
            }
            if (!ClientInitService.instance.LockInput && EnterKeyDown == 1 && EnterKeyUp == 1)
            {
                ClientInitService.instance.LockInput = true;
                MessageInput.transform.parent.parent.gameObject.SetActive(true);
                EnterKeyDown = 0;
                EnterKeyUp = 0;
                MessageInput.Select();
                MessageInput.ActivateInputField();
            }
        }

        public void BattleUIPrepare(ECSEntity battleEntity)
        {
            var simplenfo = battleEntity.GetComponent<BattleSimpleInfoComponent>();
            UnityAction<KeyValuePair<long, Command>, TeamGoalElement> generateStatsToCommand = (teamKeyVal, teamGoalElement) =>
            {
                teamStats[teamKeyVal.Key] = teamGoalElement;
                teamGoalElement.TeamId = teamKeyVal.Key;
                teamGoalElement.GetComponentsInChildren<ContentLayoutRefreshMarker>().ForEach(x => x.RegisterForUpdating());
                teamGoalElement.gameObject.SetActive(true);
            };
            UnityAction<KeyValuePair<long, Command>, TeamGoalElement, bool> colorizeStatsToCommand = (teamKeyVal, teamGoalElement, defaultcolor) =>
            {
                teamStats[teamKeyVal.Key] = teamGoalElement;
                teamGoalElement.TeamId = teamKeyVal.Key;
                var teamColor = defaultcolor ? ColorEx.ToColor(TeamColors.colors["disabled"]) : ColorEx.ToColor(TeamColors.colors[teamKeyVal.Value.TeamColor]);
                var counterOffset = TeamColors.elementColorOffsets["nicknameColor"];
                var teamStatsColor = Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
                teamStatsColor.a = 0.49f;
                teamGoalElement.StatsElementBackground.ShadeColor = teamStatsColor;
                teamGoalElement.StatsElementBackground.UpdateColor();
                counterOffset = TeamColors.elementColorOffsets["teamStatsColor"];
                var teamStatsValueColor = defaultcolor ? Color.white : Color.Lerp(teamColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
                teamGoalElement.GoalValue.color = teamStatsValueColor;
                teamGoalElement.GoalIconBackground.color = teamStatsValueColor;
                teamGoalElement.GoalIcon.color = teamStatsValueColor;
                //teamGoalElement.GetComponentsInChildren<ContentLayoutRefreshMarker>().ForEach(x => x.RegisterForUpdating());
                teamGoalElement.gameObject.SetActive(true);
            };
            TeamStatsPanel.SetActive(true);
            if (simplenfo.battleSimpleInfo.Commands.Count == 1)
            {
                var teamKeyVal = simplenfo.battleSimpleInfo.Commands.ElementAt(0);
                generateStatsToCommand(teamKeyVal, LeftTeamGoal);
                colorizeStatsToCommand(simplenfo.battleSimpleInfo.Commands.ElementAt(0), LeftTeamGoal, true);
            }
            else if (simplenfo.battleSimpleInfo.Commands.Count == 2)
            {
                generateStatsToCommand(simplenfo.battleSimpleInfo.Commands.ElementAt(0), LeftTeamGoal);
                colorizeStatsToCommand(simplenfo.battleSimpleInfo.Commands.ElementAt(0), LeftTeamGoal, false); //Instantiate(LeftTeamGoal.gameObject, LeftTeamGoal.transform.parent).GetComponent<TeamGoalElement>()
                generateStatsToCommand(simplenfo.battleSimpleInfo.Commands.ElementAt(1), LeftTeamGoal);
                colorizeStatsToCommand(simplenfo.battleSimpleInfo.Commands.ElementAt(1), RightTeamGoal, false);
            }
            BattleTimeUpdate(battleEntity);
            var battleComponent = battleEntity.GetComponent<BattleComponent>();
            BattleInfoGoalValue.text = battleComponent.BattleWinGoalValue.ToString();

            var battleResourcesCard = ResourcesService.instance.GameAssets.GetDirectory($"battle\\ui").FillChildContentToItem().GetElement<ItemCard>("card");

            BattleInfoGoalIcon.sprite = battleResourcesCard.GetElement<Sprite>(battleComponent.BattleMode.ToUpper() + "_MiniIcon");
            simplenfo.battleSimpleInfo.Commands.ForEach(x => 
            {
                teamStats[x.Key].GoalValue.text = x.Value.GoalScore.ToString();
                teamStats[x.Key].GoalIconBackground.sprite = battleResourcesCard.GetElement<Sprite>(battleComponent.BattleMode.ToUpper() + "_Background");
                teamStats[x.Key].GoalIcon.sprite = battleResourcesCard.GetElement<Sprite>(battleComponent.BattleMode.ToUpper());
                teamStats[x.Key].GoalValue.text = x.Value.GoalScore.ToString();
            });
        }

        public void BattleTimeUpdate(ECSEntity battleEntity)
        {
            var battleComponent = battleEntity.GetComponent<BattleComponent>();
            var simplenfo = battleEntity.GetComponent<BattleSimpleInfoComponent>();
            battleRoundTime = battleComponent.BattleTimeMinutes * 60;
            battleRemainingTime = System.Convert.ToSingle(simplenfo.battleSimpleInfo.TimeRemain / 1000);
            if(battleTimeTimer != null && !battleTimeTimer.isCancelled)
            {
                battleTimeTimer.Cancel();
            }
            battleTimeTimer = UnityTimer.Register(System.Convert.ToSingle(simplenfo.battleSimpleInfo.TimeRemain / 1000), () => { }, new List<System.Action<float>>(){
                (time) =>
                {
                    var handler = UIService.instance.battleUIHandler;
                    var timeLeft = (handler.battleRemainingTime - time);
                    var minutes = Mathf.Floor(timeLeft / 60);
                    var seconds = (System.Convert.ToInt32(timeLeft) - (minutes*60)).ToString();
                    BattleInfoTimeIcon.fillAmount = (handler.battleRemainingTime - time) / handler.battleRoundTime;
                    handler.BattleInfoTimeText.text = minutes.ToString() + ":" + (seconds.Length < 2 ? "0" + seconds : seconds);
                }
            });
        }

        public void TeamStatsUpdate(BattleScoreComponent battleScoreComponent)
        {
            battleScoreComponent.TeamsScore.ForEach(x =>
            {
                if(teamStats.ContainsKey(x.Key))
                {
                    teamStats[x.Key].GoalValue.text = x.Value.GoalScore.ToString();
                }
            });
        }
        public void OnBattleExit()
        {
            foreach(var statsTeam in teamStats)
            {
                //Destroy(statsTeam.Value.gameObject);
                statsTeam.Value.gameObject.SetActive(false);
            }
            teamStats.Clear();
            if(battleTimeTimer != null)
                battleTimeTimer.Cancel();
        }

        public void SendChatMessage()
        {
            if (MessageInput.text != "")
            {
                ClientNetworkService.instance.Socket.emit(new ChatSendMessageEvent()
                {
                    battleEntity = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(ClientNetworkService.instance.PlayerEntityId).NowBattleId,
                    channelEntity = 0,
                    messageBody = MessageInput.text,
                    teamMessage = false
                }.PackToNetworkPacket());
                MessageInput.text = "";
            }
        }

        public void ShowMessage(int Rank, string Nickname, string Message, long entityId)
        {
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(entityId, out var playerEntity) && playerEntity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankM))
            {
                var messageObj = Instantiate(ChatMessageExample, ChatCanvas.transform);
                messageObj.gameObject.SetActive(true);
                messageObj.UpdateMessage(Rank, Nickname, Message, tankM.NicknameColor);
                messages.Add(messageObj, entityId);
            }
        }

        public void ShowKillLog(KillEvent killEvent)
        {
            var battleManager = EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(killEvent.WhoDeadId);
            GameObject killLog = null;
            if (!battleManager.ContainsKey(killEvent.WhoDeadId) || !battleManager.ContainsKey(killEvent.WhoKilledId))
            {
                ULogger.Error("Error ShowKillLog");
                return;
            }
            var whoDead = battleManager[killEvent.WhoDeadId].GetComponent<EntityManagersComponent>().Get<TankManager>();
            if (killEvent.WhoDeadId == killEvent.WhoKilledId)
            {
                killLog = Instantiate(KilllogRowExample.gameObject, KilllogContent.transform);
                var killlogRow = killLog.GetComponent<KillLogRowElement>();
                killlogRow.DeadNickname.GetComponent<TMP_Text>().text = whoDead.tankUI.GetComponent<TankUI>().UIUsername.GetComponent<Text>().text;
                killlogRow.DeadNickname.GetComponent<TMP_Text>().color = whoDead.NicknameColor;
                killlogRow.DeadRankIcon.GetComponent<Image>().sprite = whoDead.tankUI.GetComponent<TankUI>().UIRank.GetComponent<Image>().sprite;
                killlogRow.KillInfo.GetComponent<TMP_Text>().text = "self-destruct";
                killlogRow.KillerNickname.gameObject.SetActive(false);
                killlogRow.KillerRankIcon.gameObject.SetActive(false);
                killLog.SetActive(true);
                KillLogs.Add(killLog);
            }
            if (killEvent.WhoDeadId != killEvent.WhoKilledId)
            {
                var whoKilled = battleManager[killEvent.WhoKilledId].GetComponent<EntityManagersComponent>().Get<TankManager>();
                killLog = Instantiate(KilllogRowExample.gameObject, KilllogContent.transform);
                var killlogRow = killLog.GetComponent<KillLogRowElement>();
                killlogRow.DeadNickname.GetComponent<TMP_Text>().text = whoDead.tankUI.GetComponent<TankUI>().UIUsername.GetComponent<Text>().text;
                killlogRow.DeadNickname.GetComponent<TMP_Text>().color = whoDead.NicknameColor;
                killlogRow.DeadRankIcon.GetComponent<Image>().sprite = whoDead.tankUI.GetComponent<TankUI>().UIRank.GetComponent<Image>().sprite;
                killlogRow.KillInfo.GetComponent<TMP_Text>().text = "kill";
                killlogRow.KillerNickname.GetComponent<TMP_Text>().text = whoKilled.tankUI.GetComponent<TankUI>().UIUsername.GetComponent<Text>().text;
                killlogRow.KillerNickname.GetComponent<TMP_Text>().color = whoKilled.NicknameColor;
                killlogRow.KillerRankIcon.GetComponent<Image>().sprite = whoKilled.tankUI.GetComponent<TankUI>().UIRank.GetComponent<Image>().sprite;
                killLog.SetActive(true);
                KillLogs.Add(killLog);
            }
            if(KilllogContent.transform.childCount > 10)
            {
                Destroy(KilllogContent.transform.GetChild(1).gameObject);
            }
            //KilllogRowExample.transform.parent.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }

        public void UpdateStats(ECSEntity battleEntity)
        {
            var battleSimpleInfo = battleEntity.GetComponent<BattleSimpleInfoComponent>(BattleSimpleInfoComponent.Id);
            var battleComponent = battleEntity.GetComponent<BattleComponent>(BattleComponent.Id);
            BattleInfoFundValue.text = battleSimpleInfo.battleSimpleInfo.BattleRoundFund.ToString();
            StatsMapName.GetComponent<Text>().text = battleComponent.BattleCustomName;
            List<long> toRemove = new List<long>();

            statCommands.ForEach((commandUI) => {
                if (!battleSimpleInfo.battleSimpleInfo.Commands.TryGetValue(commandUI.Key, out _))
                {
                    Destroy(commandUI.Value.gameObject);
                    toRemove.Add(commandUI.Key);
                }
            });
            toRemove.ForEach((removedKey) => statCommands.Remove(removedKey));
            toRemove.Clear();

            foreach (var command in battleSimpleInfo.battleSimpleInfo.Commands)
            {
                StatsCommandElement UICommand;
                if (!statCommands.TryGetValue(command.Key, out UICommand))
                {
                    UICommand = Instantiate(ExampleCommand, StatsWindow.transform);
                    UICommand.gameObject.SetActive(true);
                    statCommands.Add(command.Key, UICommand);
                }


                UICommand.commandStats.ForEach((playerUI) => {
                    if (!command.Value.commandPlayers.TryGetValue(playerUI.Key, out _) && playerUI.Key != 0 && playerUI.Key != 1)
                    {
                        Destroy(playerUI.Value.gameObject);
                        toRemove.Add(playerUI.Key);
                        //UICommand.commandStats.Remove(playerUI.Key);
                    }
                });
                toRemove.ForEach((removedKey) => UICommand.commandStats.Remove(removedKey));
                toRemove.Clear();

                foreach (var commandPlayer in command.Value.commandPlayers)
                {
                    StatsUserRowElement UIplayer;
                    if (!UICommand.commandStats.TryGetValue(commandPlayer.Key, out UIplayer))
                    {
                        UIplayer = Instantiate(UICommand.commandStats[1], UICommand.gameObject.transform);
                        UICommand.commandStats.Add(commandPlayer.Key, UIplayer);
                    }
                    UIplayer.Nickname.text = commandPlayer.Value.Username;
                    UIplayer.BattleScore.text = commandPlayer.Value.Score.ToString();
                    UIplayer.KilledCount.text = commandPlayer.Value.Killed.ToString();
                    UIplayer.DeathCount.text = commandPlayer.Value.Death.ToString();
                    UIplayer.KillPerDeath.text = ((commandPlayer.Value.Death == 0 || commandPlayer.Value.Killed == 0) ? "—" : System.Math.Round(((float)(commandPlayer.Value.Killed)) / ((float)(commandPlayer.Value.Death)), 2).ToString());

                    UIplayer.RankIcon.GetComponent<Image>().sprite = RankService.instance.GetRank(commandPlayer.Value.Rank).miniSprite;

                    if (commandPlayer.Value.Crystals != -1)
                    {
                        UICommand.commandStats[0].BattleReward.gameObject.transform.parent.parent.gameObject.SetActive(true);
                        UIplayer.BattleReward.gameObject.transform.parent.parent.gameObject.SetActive(true);
                        UIplayer.BattleReward.text = (commandPlayer.Value.Crystals).ToString();
                    }
                    else
                    {
                        UICommand.commandStats[0].BattleReward.gameObject.transform.parent.parent.gameObject.SetActive(false);
                        UIplayer.BattleReward.gameObject.transform.parent.parent.gameObject.SetActive(false);
                    }

                    UnityAction<KeyValuePair<long, Command>, Colorizer> colorizePlayerStatsToCommand = (teamKeyVal, teamColorControl) =>
                    {
                        var teamColor = ColorEx.ToColor(TeamColors.colors[teamKeyVal.Value.TeamColor]);
                        teamColorControl.UpdateColor(teamColor);
                    };

                    colorizePlayerStatsToCommand(command, UIplayer.TeamColor);
                    colorizePlayerStatsToCommand(command, UICommand.commandStats[0].TeamColor);

                    UIplayer.gameObject.SetActive(true);
                }

            }
        }

        public void AppendKillLog(KillEvent killEvent)
        {

        }

        public void AppendChatMessage()
        {

        }

        public void AppendBattleEvent(string eventText)
        {

        }
    }

}