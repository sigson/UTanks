using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class RigidbodyComponent : Component
    {
        public RigidbodyComponent()
        {
        }

        public RigidbodyComponent(UnityEngine.Rigidbody rigidbody)
        {
            this.Rigidbody = rigidbody;
            cacheRigidbodyAngularDrag = rigidbody.angularDrag;
            cacheRigidbodyDrag = rigidbody.drag;
        }

        public UnityEngine.Rigidbody Rigidbody { get; set; }
        public float cacheRigidbodyAngularDrag;
        public float cacheRigidbodyDrag;

        public Transform RigidbodyTransform =>
            this.Rigidbody.transform;
    }
}