using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Round
{
    [TypeUid(140660445575918460)]
    public class RoundGroupComponent : ECSComponentGroup
    {
        static public new long Id { get; set; }
    }
}
