using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class BattleSettingsHandler : MonoBehaviour
    {
        public List<GameObject> generatedSettings = new List<GameObject>();
        public Image ProIcon;
        public Image GoalIcon;
        public TMP_Text GoalValue;
        public Image TimeIcon;
        public TMP_Text TimeValue;
        public TMP_Text WeatherValue;
        public TMP_Text MapDayTime;
        public GameObject SettingsIconObjectExample;

    }
}