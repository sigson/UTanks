using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.ComponentsGroup.Garage
{
    [TypeUid(230891812243147840)]
    public class BoosterGroupComponent : ECSComponentGroup
    {
        static public new long Id { get; set; }
    }
}
