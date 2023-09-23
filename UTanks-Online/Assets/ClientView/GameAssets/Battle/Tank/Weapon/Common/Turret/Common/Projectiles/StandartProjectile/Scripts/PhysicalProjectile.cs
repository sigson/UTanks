using SecuredSpace.Effects;
using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class PhysicalProjectile : IProjectile
    {
        public Rigidbody thisRigidbody;
        public ConstantForce thisConstantForce;
        public Collider thisTriggerCollider;
        public Transform thisTransform;
        public bool isGravity;

        public void OnEnable()
        {
            UpdateStatements();
        }

        public void UpdateStatements()
        {
            thisRigidbody.useGravity = isGravity;
            checkedDistance = false;
            thisConstantForce.force = Direction * Speed;
            thisRigidbody.velocity = Direction * Speed;
            if(projectileLight != null)
            {
                if (projectileLightSize != -1)
                    projectileLight.lightSize = projectileLightSize;
                projectileLight.lightColor = projectileLightColor;
                projectileLight.UpdateLight();
                projectileLight.LightSwitch(true);
            }
            hitedTanks.Clear();
        }

        public void Update()
        {
            if (MaxDistance != float.MaxValue && MaxDistance != 0 && Vector3.Distance(this.StartPosition, thisTransform.position) >= MaxDistance && checkedDistance == false)
            {
                OnDistanceExcess(this.gameObject);
                checkedDistance = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction(this, other);
        }
    }
}