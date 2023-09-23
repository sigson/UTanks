using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class SuspensionRay
    {
        public bool hadPreviousCollision;
        public bool hasCollision;
        public RaycastHit rayHit;
        public LayerMask layerMask;
        public Vector3 surfaceVelocity;
        public Vector3 velocity;
        public float compression;
        private Vector3 origin;
        private Vector3 direction;
        private ChassisConfigComponent chassisConfig;
        private ChassisComponent chassis;
        private Vector3 globalOrigin;
        private Vector3 globalDirection;
        private float prevCompression;
        private float damping;
        private float nextRaycastUpdateTime;
        private RaycastHit lastRayHit;
        private bool lastCollision;

        public SuspensionRay(Rigidbody body, Vector3 origin, Vector3 direction, ChassisConfigComponent chassisConfig, ChassisComponent chassis, float damping)
        {
            this.rigidbody = body;
            this.origin = origin;
            this.direction = direction;
            this.chassisConfig = chassisConfig;
            this.chassis = chassis;
            this.damping = damping;
            this.ConvertToGlobal();
            this.rayHit.distance = chassisConfig.MaxRayLength;
            this.rayHit.point = this.globalOrigin + (this.globalDirection * chassisConfig.MaxRayLength);
        }

        public void ApplySpringForce(float dt)
        {
            this.compression = this.chassisConfig.MaxRayLength - this.rayHit.distance;
            float num = ((this.compression - this.prevCompression) * this.damping) / dt;
            float num2 = Mathf.Max((float)((this.chassis.SpringCoeff * this.compression) + num), (float)0f);
            this.rigidbody.AddForceAtPositionSafe(this.globalDirection * -num2, this.globalOrigin);
        }

        private void CalculateSurfaceVelocity()
        {
            this.surfaceVelocity = (this.rayHit.rigidbody == null) ? Vector3.zero : this.rayHit.rigidbody.GetPointVelocity(this.rayHit.point);
        }

        private bool ContinuousRayCast(Ray ray, out RaycastHit rayHit, float range, int layerMask, float period)
        {
            if (Time.timeSinceLevelLoad <= this.nextRaycastUpdateTime)
            {
                rayHit = this.lastRayHit;
                return this.lastCollision;
            }
            this.lastCollision = Physics.Raycast(ray, out this.lastRayHit, range, layerMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(ray.origin, ray.direction);
            //Debug.Log(ray.origin);
            rayHit = this.lastRayHit;
            this.nextRaycastUpdateTime = Time.timeSinceLevelLoad + period;
            return this.lastCollision;
        }

        private void ConvertToGlobal()
        {
            this.globalDirection = this.rigidbody.transform.TransformDirection(this.direction);
            this.globalOrigin = this.rigidbody.transform.TransformPoint(this.origin);
        }

        public Vector3 GetGlobalDirection() =>
            this.globalDirection;

        public Vector3 GetGlobalOrigin() =>
            this.globalOrigin;

        private void Raycast(float updatePeriod)
        {
            this.ConvertToGlobal();
            this.prevCompression = this.chassisConfig.MaxRayLength - this.rayHit.distance;
            this.hadPreviousCollision = this.hasCollision;
            this.hasCollision = this.ContinuousRayCast(new Ray(this.globalOrigin, this.globalDirection), out this.rayHit, this.chassisConfig.MaxRayLength, (int)this.layerMask, updatePeriod);
        }

        public void ResetContinuousRaycast()
        {
            this.nextRaycastUpdateTime = 0f;
        }

        public void Update(float dt, float updatePeriod)
        {
            this.Raycast(updatePeriod);
            if (this.hasCollision)
            {
                this.ApplySpringForce(dt);
                this.CalculateSurfaceVelocity();
                this.velocity = this.rigidbody.GetPointVelocity(this.rayHit.point);
            }
            else
            {
                this.surfaceVelocity = Vector3.zero;
                this.velocity = Vector3.zero;
                this.rayHit.distance = this.chassisConfig.MaxRayLength;
                this.rayHit.point = this.globalOrigin + (this.globalDirection * this.chassisConfig.MaxRayLength);
            }
        }

        public Rigidbody rigidbody { private get; set; }
    }
}