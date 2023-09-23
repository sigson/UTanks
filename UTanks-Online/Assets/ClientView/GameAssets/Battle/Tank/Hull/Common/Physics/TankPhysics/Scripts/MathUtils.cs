using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public static class MathUtil
    {
        private const float TWO_PI = 6.283185f;
        public static bool allowUnsafeCode = true;
        public static unsafe Quaternion AddScaledVector(Quaternion q, Vector3 v, float scale)
        {
            float num = v.x * scale;
            float num2 = v.y * scale;
            float num3 = v.z * scale;
            float num4 = ((-q.x * num) - (q.y * num2)) - (q.z * num3);
            float num5 = ((num * q.w) + (num2 * q.z)) - (num3 * q.y);
            float num6 = ((num2 * q.w) + (num3 * q.x)) - (num * q.z);
            float num7 = ((num3 * q.w) + (num * q.y)) - (num2 * q.x);
            Quaternion quaternion = new Quaternion(q.x + (0.5f * num5), q.y + (0.5f * num6), q.z + (0.5f * num7), q.w + (0.5f * num4));
            float f = (((quaternion.w * quaternion.w) + (quaternion.x * quaternion.x)) + (quaternion.y * quaternion.y)) + (quaternion.z * quaternion.z);
            if (f == 0f)
            {
                quaternion.w = 1f;
            }
            else
            {
                f = 1f / Mathf.Sqrt(f);
                Quaternion* quaternionPtr1 = &quaternion;
                quaternionPtr1->w *= f;
                Quaternion* quaternionPtr2 = &quaternion;
                quaternionPtr2->x *= f;
                Quaternion* quaternionPtr3 = &quaternion;
                quaternionPtr3->y *= f;
                Quaternion* quaternionPtr4 = &quaternion;
                quaternionPtr4->z *= f;
            }
            return quaternion;
        }

        public static float ClampAngle(float radians)
        {
            radians = radians % 6.283185f;
            return ClampAngleFast(radians);
        }

        public static float ClampAngle180(float degrees) =>
            (((degrees % 360f) + 540f) % 360f) - 180f;

        public static float ClampAngleFast(float radians) =>
            (radians >= -3.141593f) ? ((radians <= 3.141593f) ? radians : (radians - 6.283185f)) : (6.283185f + radians);

        public static Matrix4x4 FromAxisAngle(Vector3 axis, float angle)
        {
            float num = Mathf.Cos(angle);
            float num2 = Mathf.Sin(angle);
            float num3 = 1f - num;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            return new Matrix4x4
            {
                m00 = ((num3 * x) * x) + num,
                m01 = ((num3 * x) * y) - (z * num2),
                m02 = ((num3 * x) * z) + (y * num2),
                m10 = ((num3 * x) * y) + (z * num2),
                m11 = ((num3 * y) * y) + num,
                m12 = ((num3 * y) * z) - (x * num2),
                m20 = ((num3 * x) * z) - (y * num2),
                m21 = ((num3 * y) * z) + (x * num2),
                m22 = ((num3 * z) * z) + num
            };
        }

        public static int GetBitValue(int value, int bitIndex) =>
            (value >> (bitIndex & 0x1f)) & 1;

        public static Vector3 GetEulerAngles(Matrix4x4 m)
        {
            Vector3 vector;
            if ((-1f < m.m20) && (m.m20 < 1f))
            {
                vector = new Vector3(Mathf.Atan2(m.m21, m.m22), -Mathf.Asin(m.m20), Mathf.Atan2(m.m10, m.m00));
            }
            else
            {
                vector = new Vector3(0f, 0.5f * ((m.m20 > -1f) ? -3.141593f : 3.141593f), Mathf.Atan2(-m.m01, m.m11));
            }
            return (vector * 57.29578f);
        }

        public static Vector3 LocalPositionToWorldPosition(Vector3 position, GameObject gameObject) =>
            gameObject.transform.localToWorldMatrix.MultiplyPoint3x4(position);

        public static bool NearlyEqual(float a, float b, float epsilon) =>
            Mathf.Abs((float)(a - b)) < epsilon;

        public static bool NearlyEqual(Vector3 a, Vector3 b, float epsilon) =>
            (NearlyEqual(a.x, b.x, epsilon) && NearlyEqual(a.y, b.y, epsilon)) && NearlyEqual(a.z, b.z, epsilon);

        public static Matrix4x4 SetRotationMatrix(Vector3 eulerAngles)
        {
            float num = Mathf.Cos(eulerAngles.x);
            float num2 = Mathf.Sin(eulerAngles.x);
            float num3 = Mathf.Cos(eulerAngles.y);
            float num4 = Mathf.Sin(eulerAngles.y);
            float num5 = Mathf.Cos(eulerAngles.z);
            float num6 = Mathf.Sin(eulerAngles.z);
            float num7 = num5 * num4;
            float num8 = num6 * num4;
            return new Matrix4x4
            {
                m00 = num5 * num3,
                m01 = (num7 * num2) - (num6 * num),
                m02 = (num7 * num) + (num6 * num2),
                m10 = num6 * num3,
                m11 = (num8 * num2) + (num5 * num),
                m12 = (num8 * num) - (num5 * num2),
                m20 = -num4,
                m21 = num3 * num2,
                m22 = num3 * num
            };
        }

        public static float Sign(float value) =>
            (value >= 0f) ? ((value <= 0f) ? 0f : 1f) : -1f;

        public static float SignEpsilon(float value, float eps) =>
            (value >= -eps) ? ((value <= eps) ? 0f : 1f) : -1f;

        public static float Snap(float value, float snapValue, float epsilon) =>
            ((value <= (snapValue - epsilon)) || (value >= (snapValue + epsilon))) ? value : snapValue;

        public static Vector3 WorldPositionToLocalPosition(Vector3 position, GameObject gameObject) =>
            gameObject.transform.worldToLocalMatrix.MultiplyPoint3x4(position);
    }

}