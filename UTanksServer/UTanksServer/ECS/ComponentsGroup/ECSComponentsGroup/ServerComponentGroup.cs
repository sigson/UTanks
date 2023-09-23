using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.ECSComponentsGroup
{
    [TypeUid(203567232454408000)]
    public class ServerComponentGroup : ECSComponentGroup
    {
        static public new long Id { get; set; }
    }
}
