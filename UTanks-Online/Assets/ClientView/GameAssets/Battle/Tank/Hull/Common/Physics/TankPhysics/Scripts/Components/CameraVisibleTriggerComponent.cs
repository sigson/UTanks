using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class CameraVisibleTriggerComponent : MonoBehaviour
    {
        public bool IsVisibleAtRange(float testRange) =>
            this.IsVisible && (this.DistanceToCamera < testRange);

        private void OnBecameInvisible()
        {
            this.IsVisible = false;
        }

        private void OnBecameVisible()
        {
            this.IsVisible = true;
        }

        public Transform MainCameraTransform { get; set; }

        public bool IsVisible { get; set; }

        public float DistanceToCamera =>
            (this.MainCameraTransform == null) ? 0f : Vector3.Distance(this.MainCameraTransform.position, base.gameObject.transform.position);
    }
}