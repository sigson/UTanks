using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class LobbyInBattleCommandPlayerRow : MonoBehaviour
    {
        public long playerEntityId;
        public Image PlayerRankIcon;
        public Text PlayerUsername;
        public Text PlayerScore;
    }
}