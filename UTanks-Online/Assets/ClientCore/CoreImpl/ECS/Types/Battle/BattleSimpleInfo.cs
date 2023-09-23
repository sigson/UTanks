using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient;

namespace Assets.ClientCore.CoreImpl.ECS.Types.Battle
{
    public class BattleSimpleInfo : CachingSerializable
    {
        public long BattleEntityId;
        public float BattleRoundFund;
        public double TimeRemain;
        public Dictionary<long, Command> Commands = new Dictionary<long, Command>();
    }

    public class Command : CachingSerializable
    {
        public string TeamColor;
        public int GoalScore;
        public Dictionary<long, CommandPlayers> commandPlayers = new Dictionary<long, CommandPlayers>();
    }

    public class CommandPlayers : CachingSerializable
    {
        public int Place;
        public int Rank;
        public string Username;
        public long EntityId;
        public int Score;
        public int Killed;
        public int Death;
        public int Crystals;
    }
}
