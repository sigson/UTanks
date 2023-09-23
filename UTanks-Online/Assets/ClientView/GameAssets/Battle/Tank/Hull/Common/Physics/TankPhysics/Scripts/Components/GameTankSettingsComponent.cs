using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class GameTankSettingsComponent : Component
    {
        public bool MovementControlsInverted { get; set; }

        public bool DamageInfoEnabled { get; set; }

        public bool HealthFeedbackEnabled { get; set; }

        public bool SelfTargetHitFeedbackEnabled { get; set; }
    }
}