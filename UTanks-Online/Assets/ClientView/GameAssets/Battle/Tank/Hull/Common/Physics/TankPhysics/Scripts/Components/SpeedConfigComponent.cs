using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class SpeedConfigComponent : Component
    {
        public float ReverseAcceleration { get; set; }

        public float SideAcceleration { get; set; }

        public float TurnAcceleration { get; set; }

        public float ReverseTurnAcceleration { get; set; }
    }
}