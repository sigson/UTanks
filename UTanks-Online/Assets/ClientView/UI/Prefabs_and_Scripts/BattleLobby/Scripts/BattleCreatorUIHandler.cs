using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.Controls;
using SecuredSpace.UI.Special;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Types.Lobby;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.GameUI
{
    public class BattleCreatorUIHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField MapCustomName;
        [SerializeField] private TMP_Dropdown MapSelector;
        [SerializeField] private TMP_Dropdown MapTypeSelector;
        [SerializeField] private Image MapCreatorPreviewImage;
        [SerializeField] private GameObject MapGameModePattern;
        [SerializeField] private InputField MaxPlayersCounter;
        [SerializeField] private InputField BattleTimeCounter;
        [SerializeField] private InputField BattleGoal;
        [SerializeField] private Image BattleGoalIcon;
        [SerializeField] private Text BattleGoalText;
        public RadioSelectorHandler BattleModeHandler;

        [Space(10)]
        [Header("BattleCreatorUISettingsPage1")]
        [SerializeField] private Toggle IsProBattle;
        [SerializeField] private Toggle IsPrivateBattle;
        [SerializeField] private Toggle BattleEnableUpgrades;
        [SerializeField] private Toggle BattleEnableDevices;
        [SerializeField] private Toggle BattleEnableDrones;
        [SerializeField] private Toggle BattleEnableResists;
        [SerializeField] private Toggle BattleEnableTacticModules;
        [SerializeField] private Toggle BattleEnablePlayerSupplies;
        [SerializeField] private Toggle BattleEnableAutobalance;
        [SerializeField] private Toggle BattleEnableAutoending;
        [SerializeField] private Toggle BattleEnableFriendlyFire;
        [SerializeField] private Toggle BattleEnableDressUp;
        [SerializeField] private Toggle BattleEnableDependentSupplies;
        [SerializeField] private Toggle BattleEnableSupplyDrop;
        [SerializeField] private Toggle BattleEnableCrystalDrop;
        [SerializeField] private Toggle BattleEnableFairGoldbox;
        [SerializeField] private Toggle BattleEnableTestboxes;
        [SerializeField] private Toggle BattleEnableDestroyableMap;
        [Space(10)]
        [Header("BattleCreatorUISettingsPage2")]
        [SerializeField] private Toggle BattleEnableCheatMode;
        [SerializeField] private Toggle BattleEnableMidgetMode;
        [SerializeField] private Toggle IsParkourBattle;
        [SerializeField] private Toggle BattleEnableInfinitySupplies;
        [SerializeField] private Toggle BattleEnableAlternateSupplyActivation;
        [SerializeField] private Toggle BattleEnableBotsFilling;
        [SerializeField] private Slider WeatherSlider;
        [SerializeField] private Slider TimeSlider;
        [Space(10)]
        [SerializeField] private Button[] FinallyCreateBattleButton;

        private Dictionary<string, OrderedDictionary<string, MapValue>> CacheMapValueDB = new Dictionary<string, OrderedDictionary<string, MapValue>>();
        public void StartImpl()
        {
            FinallyCreateBattleButton.ForEach(x => x.onClick.AddListener(OnFinallyCreateBattleButton));
            MapGameModePattern.SetActive(false);
            GlobalGameDataConfig.SelectableMap.selectableMaps.GameMaps.ForEach((mapGroup) =>
            {
                MapSelector.options.Add(new TMP_Dropdown.OptionData(mapGroup.Name));
                CacheMapValueDB.Add(mapGroup.Name, new OrderedDictionary<string, MapValue>().Fill(mapGroup.Maps, (dict, mapval) => dict[(mapval as MapValue).MapGroupName] = (mapval as MapValue)));
            });
            BattleModeHandler.OnChangeSelected += OnChangeBattleMode;
            UnityAction<int> unityAction = (newValue) => {
                MapTypeSelector.options.Clear();
                CacheMapValueDB[MapSelector.options[newValue].text].ForEach((mapGroupElem) =>
                MapTypeSelector.options.Add(new TMP_Dropdown.OptionData(mapGroupElem.Value.MapGroupName)));
                MapTypeSelector.value = 0;
                MapTypeSelector.captionText.text = MapTypeSelector.options[0].text;
                OnSelectMapType(0);
            };
            
            MapSelector.onValueChanged.AddListener(unityAction);
            MapTypeSelector.onValueChanged.AddListener(OnSelectMapType);
            unityAction(MapSelector.value);
        }

        private void OnSelectMapType(int newValue)
        {
            MapValue mapValue = CacheMapValueDB[MapSelector.options[MapSelector.value].text][MapTypeSelector.options[newValue].text];
            MaxPlayersCounter.GetComponent<InputFieldValueTreshold>().Max = mapValue.MaxPlayers;
            MapGameModePattern.transform.parent.GetComponentsInChildren<RadioSelector>().ForEach(x => x.gameObject.SetActive(false));
            foreach (var battleMode in mapValue.BattleModes)
            {
                RadioSelector radioBattleMode = null;
                if (!BattleModeHandler.radioVariants.TryGetValue(battleMode, out radioBattleMode))
                {
                    var newBattleMode = Instantiate(MapGameModePattern, MapGameModePattern.transform.parent);
                    radioBattleMode = newBattleMode.GetComponent<RadioSelector>();
                    radioBattleMode.Value = battleMode.ToUpper();
                    BattleModeHandler.radioVariants[battleMode] = radioBattleMode;
                }
                radioBattleMode.gameObject.SetActive(true);
            }
            if (!Lambda.TryExecute(() => MapCreatorPreviewImage.sprite = ResourcesService.instance.GameAssets.GetDirectory("maps\\ui\\preview\\res").GetChildFSObject($"{mapValue.MapHeaderName}_{mapValue.MapVersion}_{mapValue.MapGroup}").GetContent<Sprite>()))
                MapCreatorPreviewImage.sprite = null;
            BattleModeHandler.radioVariants[mapValue.BattleModes[0]].SelectValue();
        }

        public void OnChangeBattleMode(RadioSelectorHandler selectorHandler)
        {
            var battleIconsCard = ResourcesService.instance.GameAssets.GetDirectory("battle\\ui").GetChildFSObject("card").GetContent<ItemCard>();
            var battleIcon = battleIconsCard.GetElement<Sprite>(selectorHandler.SelectedValue.GetValue<string>().ToUpper() + "_Background");
            BattleGoalIcon.sprite = battleIcon;
            BattleGoalText.GetAdapter().text = "Goal";
        }

        public void OnFinallyCreateBattleButton()
        {
            var mapConfigObject = GlobalGameDataConfig.SelectableMap.selectableMaps.GameMaps.Where(x => x.Name == MapSelector.options[MapSelector.value].text).ToList()[0].Maps.Where(x => x.MapGroupName == MapTypeSelector.options[MapTypeSelector.value].text).ToList()[0];
            var createBattleEvent = new CreateBattleEvent()
            {
                BattleMode = BattleModeHandler.SelectedValue.GetValue<string>().ToLower(),
                BattleCustomName = MapCustomName.text,
                BattleRealName = MapSelector.options[MapSelector.value].text,
                GameMapGroupName = MapSelector.options[MapSelector.value].text,
                MapPath = mapConfigObject.Path,
                MapConfigPath = mapConfigObject.MapConfigPath,
                MapModel = mapConfigObject.MapModel,
                BattleTimeMinutes = int.Parse(EmptyTextToNil(BattleTimeCounter.text)),
                BattleWinGoalValue = int.Parse(EmptyTextToNil(BattleGoal.text)),
                MaxPlayers = int.Parse(EmptyTextToNil(MaxPlayersCounter.text)),
                WeatherMode = WeatherSlider.transform.parent.parent.parent.parent.GetChild(1).GetChild(0).GetChild(Convert.ToInt32(WeatherSlider.value)).name,
                TimeMode = Convert.ToInt32(TimeSlider.value),
                enableCrystalDrop = BattleEnableCrystalDrop.isOn,
                enableDressingUp = BattleEnableDressUp.isOn,
                enablePlayerSupplies = BattleEnablePlayerSupplies.isOn,
                enablePlayerAutoBalance = BattleEnableAutobalance.isOn,
                enableResists = BattleEnableResists.isOn,
                enableSupplyDrop = BattleEnableSupplyDrop.isOn,
                enableSuperDrop = true,
                MinimalPlayerRankValue = 1,
                MaximalPlayerRankValue = 30
            };
            TaskEx.RunAsync(() =>
            {
                ClientNetworkService.instance.Socket.emit(createBattleEvent.PackToNetworkPacket());
            });
        }

        string EmptyTextToNil(string txt)
        {
            if (txt == "")
                return "0";
            return txt;
        }
    }
}