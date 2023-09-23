using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects.Lighting
{
    public class ReflectProbeLight : ILight
    {
        private ReflectionProbe reflectionProbe = null;

        private ReflectionProbe getReflectionProbe()
        {
            if (reflectionProbe == null)
                reflectionProbe = this.GetComponent<ReflectionProbe>();
            return reflectionProbe;
        }

        protected override void OnEnableImpl()
        {
            base.OnEnableImpl();
            getReflectionProbe().refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
        }

        public override void UpdateLight()
        {
            base.UpdateLight();
            getReflectionProbe().backgroundColor = lightColor;
            getReflectionProbe().blendDistance = lightSize;
            getReflectionProbe().importance = lightImportance;
            getReflectionProbe().intensity = lightIntencity;
            getReflectionProbe().RenderProbe();
        }

        protected override void OnDisableImpl()
        {
            base.OnDisableImpl();
            getReflectionProbe().refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        }

        public override void LightSwitch(bool switchValue, bool nestedUpdate = false)
        {
            getReflectionProbe().enabled = switchValue;
            getReflectionProbe().RenderProbe();
            if (!nestedUpdate)
                base.LightSwitch(switchValue);
        }
    }
}