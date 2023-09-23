using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class ChassisConfigComponent : Component
    {
        public bool ReverseBackTurn { get; set; }

        public float TrackSeparation { get; set; }

        public float SuspensionRayOffsetY { get; set; }

        public int NumRaysPerTrack { get; set; }

        public float MaxRayLength { get; set; }

        public float NominalRayLength { get; set; }

        public LayerMask trackLayerMask { get; set; }
    }
}
