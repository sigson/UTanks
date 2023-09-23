using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.User
{
    [TypeUid(213432061749023170)]
    public class TimeoutEntitySelfRemoveComponent : ECSComponent
    {
        public static new long Id = 213432061749023170;
        public long TimeDestruction;
    }
}
