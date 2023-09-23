using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class StatsUserRowElement : MonoBehaviour
    {
        public Text Nickname;
        public Image RankIcon;
        public Text BattleScore;
        public Text KilledCount;
        public Text DeathCount;
        public Text KillPerDeath;
        public Text BattleReward;
        public Colorizer TeamColor;
    }
}