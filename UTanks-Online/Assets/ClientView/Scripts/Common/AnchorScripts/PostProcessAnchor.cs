using SecuredSpace.Effects;
using SecuredSpace.Effects.SSAO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Settings
{
    public class PostProcessAnchor : IAnchor
    {
        protected override void StartImpl()
        {
            base.StartImpl();
            var lightShafts = this.GetComponent<LightShafts>();
            if (lightShafts != null)
            {
                lightShafts.m_Cameras[0] = ClientInitService.instance.NowMainCamera.GetComponent<Camera>();
                lightShafts.m_CurrentCamera = lightShafts.m_Cameras[0];
            }
        }
        protected override IEnumerator UpdateObjectImpl()
        {
            var ssaoComponent = this.GetComponent<SimpleScreenSpaceAmbientOcclusion>();
            if (ssaoComponent != null && ClientInitService.instance.gameSettings.SSAO != ssaoComponent.enabled)
                ssaoComponent.enabled = ClientInitService.instance.gameSettings.SSAO;
            var lightShafts = this.GetComponent<LightShafts>();
            if (lightShafts != null)
            {
                lightShafts.m_Cameras[0] = ClientInitService.instance.NowMainCamera.GetComponent<Camera>();
                lightShafts.m_CurrentCamera = lightShafts.m_Cameras[0];
            }
            if (lightShafts != null && ClientInitService.instance.gameSettings.VolumetricLighting != lightShafts.enabled)
                lightShafts.enabled = ClientInitService.instance.gameSettings.VolumetricLighting;
            yield return new WaitForEndOfFrame();
        }
    }
}
