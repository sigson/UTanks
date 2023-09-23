using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class Track
    {
        public SuspensionRay[] rays;
        public float animationSpeed;
        public int side;
        public int numContacts;
        public Vector3 averageSurfaceVelocity = Vector3.zero;
        public Vector3 averageVelocity = Vector3.zero;

        public Track(Rigidbody rigidbody, int numRays, Vector3 trackCenterPosition, float trackLength, ChassisConfigComponent chassisConfig, ChassisComponent chassis, int side, float damping)
        {
            this.side = side;
            this.CreateSuspensionRays(rigidbody, numRays, trackCenterPosition, trackLength, chassisConfig, chassis, damping);
        }

        private void CreateSuspensionRays(Rigidbody rigidbody, int numRays, Vector3 trackCenterPosition, float trackLength, ChassisConfigComponent chassisConfig, ChassisComponent chassis, float damping)
        {
            this.rays = new SuspensionRay[numRays];
            float num = trackLength / ((float)(this.rays.Length - 1));
            for (int i = 0; i < numRays; i++)
            {
                Vector3 origin = new Vector3(trackCenterPosition.x, trackCenterPosition.y, (trackCenterPosition.z + (0.5f * trackLength)) - (i * num));
                this.rays[i] = new SuspensionRay(rigidbody, origin, Vector3.down, chassisConfig, chassis, damping);
            }
        }

        public void SetAnimationSpeed(float targetValue, float delta)
        {
            float num;
            if (this.animationSpeed < targetValue)
            {
                num = this.animationSpeed + delta;
                this.animationSpeed = (num <= targetValue) ? num : targetValue;
            }
            else if (this.animationSpeed > targetValue)
            {
                num = this.animationSpeed - delta;
                this.animationSpeed = (num >= targetValue) ? num : targetValue;
            }
        }

        public void SetRayñastLayerMask(LayerMask layerMask)
        {
            for (int i = 0; i < this.rays.Length; i++)
            {
                this.rays[i].layerMask = layerMask;
            }
        }

        public void UpdateRigidbody(Rigidbody rigidbody)
        {
            foreach (SuspensionRay ray in this.rays)
            {
                ray.rigidbody = rigidbody;
            }
        }

        public bool UpdateSuspensionContacts(float dt, float updatePeriod)
        {
            this.numContacts = 0;
            this.averageSurfaceVelocity = Vector3.zero;
            this.averageVelocity = Vector3.zero;
            bool flag = true;
            for (int i = 0; i < this.rays.Length; i++)
            {
                SuspensionRay ray = this.rays[i];
                ray.Update(dt, updatePeriod);
                flag &= ray.hasCollision;
                if (ray.hasCollision)
                {
                    this.numContacts++;
                    this.averageSurfaceVelocity += ray.surfaceVelocity;
                    this.averageVelocity += ray.velocity;
                }
            }
            if (!flag && (updatePeriod > 0f))
            {
                for (int j = 0; j < this.rays.Length; j++)
                {
                    this.rays[j].ResetContinuousRaycast();
                }
            }
            if (this.numContacts > 1)
            {
                this.averageSurfaceVelocity /= (float)this.numContacts;
                this.averageVelocity /= (float)this.numContacts;
            }
            return flag;
        }

        public static UnityTime UnityTime { get; set; }
    }
}