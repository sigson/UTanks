using SecuredSpace.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Settings
{
    public class PropAnchor : IAnchor
    {
        public Dictionary<string, Material> PropMaterials = new Dictionary<string, Material>();
        public bool GrassProp = false;
        protected override void StartImpl()
        {
            base.StartImpl();
        }
        protected override IEnumerator UpdateObjectImpl()
        {
            var renderer = this.GetComponent<Renderer>();
            if(renderer != null)
            {
                if (ClientInitService.instance.gameSettings.EnableShadow)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            if (GrassProp)
            {
                var grassTessel = this.GetComponent<GrassTesselation>();
                if(grassTessel.GrassEnabled != ClientInitService.instance.gameSettings.Grass)
                {
                    grassTessel.GrassEnabled = ClientInitService.instance.gameSettings.Grass;
                    grassTessel.enabled = ClientInitService.instance.gameSettings.Grass;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

}