using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.UI.GameUI
{
    public class LobbyInBattlePlayerCommandTable : MonoBehaviour
    {
        public long commandEntityId;
        public GameObject InBattlePlayersCanvas;
        public LobbyInBattleCommandPlayerRow InBattlePlayerRowElementPattern;
        public Dictionary<long, LobbyInBattleCommandPlayerRow> CommandPlayersListRepresentation = new Dictionary<long, LobbyInBattleCommandPlayerRow>();
        public string Color;
    }
}