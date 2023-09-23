using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects.Lighting
{
    public class PointLight : ILight
    {
        private Light pointLight = null;

        private Light getPointLight()
        {
            if (pointLight == null)
                pointLight = this.GetComponent<Light>();
            return pointLight;
        }

        protected override void OnEnableImpl()
        {
            base.OnEnableImpl();
        }

        public override void UpdateLight()
        {
            base.UpdateLight();
            getPointLight().color = lightColor;
            getPointLight().range = lightSize;
            getPointLight().intensity = lightIntencity;
        }

        protected override void OnDisableImpl()
        {
            base.OnDisableImpl();
        }

        public override void LightSwitch(bool switchValue, bool nestedUpdate = false)
        {
            getPointLight().enabled = switchValue;
            if(!nestedUpdate)
                base.LightSwitch(switchValue);
        }
    }
}