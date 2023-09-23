using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UTanksClient.ECS.Types.Battle
{
    public class QuaternionS : CachingSerializable
    {
        public float x = 0f;
        public float y = 0f;
        public float z = 0f;
        public float w = 0f;

        public QuaternionS() { }

        public QuaternionS(Quaternion quaternion)
        {
            this.x = quaternion.x;
            this.y = quaternion.y;
            this.z = quaternion.z;
            this.w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
