using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects.Lighting
{
    public class ParticlesLight : ILight
    {
        private ParticleSystem particleLight = null;

        private ParticleSystem getParticleLight()
        {
            if (particleLight == null)
                particleLight = this.GetComponent<ParticleSystem>();
            return particleLight;
        }

        protected override void OnEnableImpl()
        {
            base.OnEnableImpl();
        }

        public override void UpdateLight()
        {
            base.UpdateLight();
            getParticleLight().lights.light.color = lightColor;
            getParticleLight().lights.light.range = lightSize;
            getParticleLight().lights.light.intensity = lightIntencity;
        }

        protected override void OnDisableImpl()
        {
            base.OnDisableImpl();
        }

        public override void LightSwitch(bool switchValue, bool nestedUpdate = false)
        {
            var lightModule = getParticleLight().lights;
            lightModule.enabled = switchValue;
            if (!nestedUpdate)
                base.LightSwitch(switchValue);
        }
    }
}