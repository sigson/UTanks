using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(214497538733962850)]
    public class BattleDropStorageComponent : ComponentsDBComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

    }
}
