using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class TankCollidersComponent : Component
    {
        public BoxCollider BoundsCollider { get; set; }

        //public BoxCollider SideDamperCollider { get; set; }

        public Collider TankToTankCollider { get; set; }

        public Collider TankToStaticTopCollider { get; set; }

        public List<GameObject> TargetingColliders { get; set; }

        public List<GameObject> VisualTriggerColliders { get; set; }

        public List<Collider> TankToStaticColliders { get; set; }

        public Vector3 Extends { get; set; }
    }
}