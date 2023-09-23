using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class TeamGoalElement : MonoBehaviour
    {
        public TMP_Text GoalValue;
        public GameObject GoalBlock;
        public Image GoalIconBackground;
        public Image GoalIcon;
        public Colorizer StatsElementBackground;
        public long TeamId;

        private void OnDisable()
        {
            GoalBlock.SetActive(false);
            StatsElementBackground.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GoalBlock.SetActive(true);
            StatsElementBackground.gameObject.SetActive(true);
        }
    }
}