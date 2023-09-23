using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class JetManager : MonoBehaviour
    {
        public float cacheEmission = 0;
        public ParticleSystem Jet;

        public void SaveState()
        {
            cacheEmission = Jet.emission.rateOverTime.constant;
        }

        public void ShowJet()
        {
            var emission = Jet.emission;
            emission.rateOverTime = cacheEmission;
        }

        public void HideJet()
        {
            var emission = Jet.emission;
            emission.rateOverTime = 0;
        }
    }
}
