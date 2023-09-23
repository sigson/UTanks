using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.ComponentsGroup.Garage
{
    [TypeUid(191072864118790080)]
    public class TurretGroupComponent : ECSComponentGroup
    {
        static public new long Id { get; set; }
    }
}
