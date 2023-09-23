using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UTanksClient.ECS.Types.Battle
{
    public class Vector3S : CachingSerializable
    {
        public float x = 0f;
        public float y = 0f;
        public float z = 0f;

        public Vector3S() { }

        public Vector3S(Vector3 vector3) 
        {
            this.x = vector3.x;
            this.y = vector3.y;
            this.z = vector3.z;
        }

        public Vector3S(float X, float Y, float Z)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }

        public Vector3 ConvertToUnityVector3()
        {
            return new Vector3(x, y, z);
        }

        public Vector3 ConvertToUnityVector3Constant007Scaling()
        {
            return new Vector3(x*0.007f, y * 0.007f, z * 0.007f);
        }

        public static Vector3S ConvertToVector3SUnScaling(Vector3 vector3N, float unscaler)
        {
            return new Vector3S(vector3N.x / unscaler, vector3N.y / unscaler, vector3N.z / unscaler);
        }

        public static Vector3S ConvertToVector3SScaling(Vector3 vector3N, float scaler)
        {
            return new Vector3S(vector3N.x * scaler, vector3N.y * scaler, vector3N.z * scaler);
        }
    }
}
