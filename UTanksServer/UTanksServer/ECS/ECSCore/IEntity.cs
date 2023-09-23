using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.ECS.ECSCore
{
    interface IEntity
    {
        [ServerOnlyData] public Dictionary<ECSEntityGroup, bool> ECSComponentGroups { get; set; }

        static long Id { get; }

        long InstanceId { get; }

        string Name { get; set; }

        string ConfigPath { get; }

        bool Alive { get; }
    }
}
