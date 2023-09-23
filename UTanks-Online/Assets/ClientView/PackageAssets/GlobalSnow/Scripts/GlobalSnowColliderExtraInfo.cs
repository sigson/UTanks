using UnityEngine;
using System.Collections;
using SecuredSpace.Battle.Tank;
using SecuredSpace.Important.TPhysics;
using System;
using System.Linq;
using SecuredSpace.Important.Raven;

namespace GlobalSnowEffect {
    public class GlobalSnowColliderExtraInfo : MonoBehaviour {

        [Tooltip("Snow mark size for this collider.")]
        public float markSize = 0.25f;

		[Tooltip("Distance between collisions")]
        public float collisionDistanceThreshold = 0.1f;

		[Tooltip("If enabled, only a single stamp will be left on the trail, instead of a continuous line, when the character moves.")]
        public bool isFootprint;

		[Tooltip("Maximum distance for a trail segment. If set to 0, marks will be added only on collision points and no interpolation between old and new position will be computed.")]
        public float stepMaxDistance = 20f;

		[Tooltip("Rotation degrees to trigger a terrain mark.")]
        public float rotationThreshold = 3f;

        [Tooltip("Enable this option to ignore any collision on the snow caused by this collider")]
        public bool ignoreThisCollider;

        public TankChassisManager chassisManager; 

        void Start() {
            chassisManager = GetComponentInParent<TankManager>().Hull.GetComponent<TankChassisManager>();
        }

        void Update() {
            GlobalSnow snow = GlobalSnow.instance;
            if (snow == null) return;

            var rContactPoints = chassisManager.chassisNode.track.RightTrack.rays;
            var lContactPoints = chassisManager.chassisNode.track.LeftTrack.rays;
            Action<SuspensionRay[]> forAction = (SuspensionRay[] contactPoints) =>
            {
                //for (int k = 0; k == 0 || k > contactPoints.Length - 2; k++)
                for (int k = 0; k < contactPoints.Length - 2; k++)
                {
                    var contact = contactPoints[k];
                    if (contact.hasCollision)
                    {
                        snow.CollisionStay(this.gameObject, null, contact.rayHit.point);
                    }
                    else
                    {
                        snow.CollisionStop(this.gameObject);
                    }
                }
            };
            forAction(lContactPoints);
            forAction(rContactPoints);
            
        }
    }
}

