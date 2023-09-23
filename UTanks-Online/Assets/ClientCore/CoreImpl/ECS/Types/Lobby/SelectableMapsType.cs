using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace UTanksClient.ECS.Types.Lobby
{
    public class SelectableMapsType
    {
        public float FundTimeSection;
        public float FundActionPerTimeSection;
        public float KillUnconditionalFundReward;
        public float KillRankKoefReward;
        public float KillUncoditionalScorePoints;
        public float KillRankCoefScorePoints;
        public float KillUncoditionalInBattleScorePoints;
        public float FlagReturnReward;
        public float FlagCapturedReward;
        public float UncapturePoint;
        public float CapturePoint;
        public float MaxPlayerSupplySeparation;
        public string DropBonusSkinPath;
        public float SupplyDropBonusDespawnSecTime;
        public float CrystalDropBonusDespawnSecTime;
        public float GoldDropBonusDespawnSecTime;
        public float RubyDropBonusDespawnSecTime;
        public float CrystalDropBonusReward;
        public float GoldDropBonusReward;
        public float RubyDropBonusReward;
        public string ContainerDropPath;
        public string SuperContainerDropPath;
        public List<MapGroup> GameMaps;
    }

    public class MapGroup
    {
        public string Name;
        public List<MapValue> Maps;
    }

    public class MapValue
    {
        public string MapHeaderName;
        public string MapGroupName;
        public string MapGroup;
        public string SkyboxName;
        public string AudioName;
        public string Path;
        public string MapModel;
        public string MapConfigPath;
        public string MapVersion;
        public List<string> BattleModes;
        public int MaxPlayers;
        public int MinimalRankAccess;
        public float FundScaling;
        public float CrystalDropFrequencyScaling;
        public float SuperDropFrequencyScaling;
        public float SupplyDropFrequencyScaling;
        public Dictionary<string, List<WorldPoint>> SpawnPoints = new Dictionary<string, List<WorldPoint>>(); //key is command/battlemode group
        public Dictionary<string, List<BonusDropRegion>> DropSpawnPoints = new Dictionary<string, List<BonusDropRegion>>(); //key is drop group
        public Dictionary<string, Dictionary<string, List<WorldPoint>>> GoalPositionPoints = new Dictionary<string, Dictionary<string, List<WorldPoint>>>(); //key is ctf/cp/ect, key 2 is group red/blue
        public Map map; //raw map
    }

    public class BonusDropRegion
    {
        public string Name;
        public string BonusType;
        public bool Parachute;
        public Vector3S Min;
        public Vector3S Max;
        public WorldPoint worldPosition;
        public double DroppingPeriod;
    }
}
