using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.ECS.Types.Battle
{
    public class Vector3S : CachingSerializable
    {
        public float x = 0f;
        public float y = 0f;
        public float z = 0f;
    }
}
