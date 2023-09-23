using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.ComponentsGroup.Garage
{
    [TypeUid(170537434368155650)]
    public class HullGroupComponent : ECSComponentGroup
    {
        static public new long Id { get; set; }
    }
}
