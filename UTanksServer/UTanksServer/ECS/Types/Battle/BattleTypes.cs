using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;

namespace UTanksServer.ECS.Types.Battle
{
    public class BattleTypes
    {
        public static Dictionary<string, Type> Modes = new Dictionary<string, Type>
        {
            {"dm",  typeof(DMComponent)},
            {"dom",  typeof(DOMComponent)},
            {"tdm",  typeof(TDMComponent)},
            {"ctf",  typeof(CTFComponent)}
        };
    }
}
