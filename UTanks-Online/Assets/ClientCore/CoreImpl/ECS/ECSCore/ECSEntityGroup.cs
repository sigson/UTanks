using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksClient.ECS.ECSCore
{
    public abstract class ECSEntityGroup
    {
        public long InstanceId = Guid.NewGuid().GuidToLong();
    }
}
