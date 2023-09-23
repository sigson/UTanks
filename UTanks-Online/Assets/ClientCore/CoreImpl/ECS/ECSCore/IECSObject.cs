using SecuredSpace.ClientControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksClient.ECS.ECSCore
{
    public class IECSObject
    {
        public long instanceId = Guid.NewGuid().GuidToLong();
        public List<IManager> connectPoints = new List<IManager>();
    }
}
