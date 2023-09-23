using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class ChassisSmootherComponent : Component
    {
        public SimpleValueSmoother maxSpeedSmoother = new SimpleValueSmoother(1f, 10f, 0f, 0f);
        public SimpleValueSmoother maxTurnSpeedSmoother = new SimpleValueSmoother(17.18873f, 10f, 0f, 0f);
    }
}