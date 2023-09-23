using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.DBResources
{
    public class PrefabID : MonoBehaviour
    {
        [SerializeField] private string _ID = ""; // for dynamic object as WeaponScripts prefab's or CreatureScripts
        public string ID
        {
            get
            {
                return _ID != "" ? _ID : this.gameObject.name;
            }
        }

        public PrefabsID cID; //only for constant base objects, like ui or camera or tank base object

        public enum PrefabsID
        {
            None,
            #region UIPrefabs

            BattleUI,
            BattleLobbyUI,
            GarageUI,
            LobbyChatUI,
            PlayerPanelUI,
            SettingsWindowUI,
            DailyMissionsUI,
            DailyGiftsUI,
            MicroUpgradesUI,
            SettingsUI,
            LoaderUI,
            BackgroundUI,
            LoginUI,
            DialogUI,
            EventSystemUI,

            #endregion
            #region GamePrefabs

            Tank,
            MapBuilder,
            TankBattleUI,
            BattleCamera,
            LobbyCamera,
            Creature,
            Drop,
            BattleInteractive

            #endregion
        }
    }
}