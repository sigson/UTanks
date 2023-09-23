using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle
{
    [TypeUid(209421267805011940)]
    public class CreateBattleEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public string BattleCustomName = "";
        public string BattleRealName = "";
        public string GameMapGroupName = ""; //Noise or Silence, without detalize summer or any mods
        public string MapPath = "";
        public string MapModel = "";
        public string MapConfigPath = "";
        public int MaxPlayers = 0;
        public int BattleTimeMinutes = 0;
        public int BattleWinGoalValue = 0;
        public int MinimalPlayerRankValue = 0;
        public int MaximalPlayerRankValue = 0;
        public string WeatherMode = "";
        public int TimeMode = 0;
        public string BattleMode = "";
        public List<string> ListOfAcceptedConfigPathWeapon = new List<string>(); //if length = 0 - all accepted
        public List<string> ListOfAcceptedConfigPathHull = new List<string>();
        public bool isProBattle = false;
        public bool isClosedBattle = false;
        public bool isParkourBattle = false;
        public bool isTournamentBattle = false; //auto up all grades to full and set fund scaler to 0
        public bool enableUnlimitedUserSupply = false; //only for owners of subscription(maybe only payment access)
        public bool enableSuperDrop = false; //golds will be disabled and auto added to fund if he was will drop
        public bool enableAutoPeaceOnSuperDrop = false; //disabling all damage after attention and turn it on after drop was dropped
        public bool enableMicroUpgrade = false;
        public bool enableDressingUp = false;
        public int dressingUpTimeoutSeconds = 240;
        public bool enableSupplyDrop = false;
        public bool enableCrystalDrop = false;
        public bool enablePlayerSupplies = false;
        public bool enablePlayerAutoBalance = false;
        public bool enableBattleAutoEnding = false;
        public bool enableTeamKilling = false;
        public bool enableResists = false;
        public bool enableModules = false;
        public bool enablePlayerSupplySeparation = false;
        public bool enableSupplyCooldown = true;

        public float LuminosityStrength = 0; //0-0.5-1
        public float DamageScalingCoeficient = 1f;
        public float HealthScalingCoeficient = 1f;
        public float GravityScaling = 1f;
        public float MassScaling = 1f;
        public bool isTestBoxBattle = false;
        public bool isCheatersBattle = false;

        public override void Execute()
        {
            //
        }
    }
}
