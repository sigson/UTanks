using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SecuredSpace.Effects.SSAO
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu("Image Effects/Rendering/Simple Screen Space Ambient Occlusion")]
    public class SimpleScreenSpaceAmbientOcclusion : MonoBehaviour
    {
        public enum VisualizationMode{
            None = 0,
            ViewAO = 1,
            ViewBleeding = 2
        }    

        public enum Quality
        {
            Low,
            Medium,
            High,
            Ultrahigh
        }

        private Camera cam;
        public Texture2D m_AxisPattern;

        [Tooltip("Tints occlusion color.")]
        public Color m_OcclusionColor = Color.black;

        [Tooltip("Quality of the SSAO effect. Higher qualities will consume more resources.")]
        public Quality m_Quality = Quality.High;

        [Tooltip("Maximum reach of the effect in world space.")]
        [Range(0.0f, 10f)]
        public float m_Radius = 0.2f;

        [Tooltip(" Minimum and maximum radius in pixels. Since the radius is scaled by pixel depth, "+
                 "pixels further away from the camera might get a very small radius and pixels "+
                 "very close to the camera might get a very big radius."+
                 "Changing this parameter clamps the radius so that it doesn´t get too small or too big at extreme distances.")]
        [MinMaxRangeAttribute(0.0001f, 0.5f)]
        public Vector2 m_RadiusRange = new Vector2(0.02f,0.3f);

        [Tooltip("Width of the occlusion cone considered by each pixel."+
                 "Set it higher to reduce self-occlusion.")]
        [Range(0.0f, 1.0f)]
        public float m_OcclusionBias = 0.05f;

        [Tooltip("Amount of base occlusion. Increasing its value will cause flat surfaces to turn grey,"+
                 " occlusion will be subtracted from  corners and added to crevices, resulting in increased contrast.")]
        [Range(0.0f, 1.0f)]
        public float m_OcclusionOffset = 0f;

        [Tooltip("Modulates the amount of occlusion contributed by each pixel.")]
        [Range(0.0f, 20.0f)]
        public float m_OcclusionIntensity = 2f;

        [Tooltip("Use this to control the shape of the occlusion curve. "+
                 "A value of 1 means linear occlusion, 2 means quadratic occlusion, and so forth.")]
        [Range(0.25f, 10.0f)]
        public float m_OcclusionExponent = 2f;

        [Tooltip("Modulates the amount of occlusion added at high luminance zones. This prevents brightly iluminated areas from being washed off by SSAO.")]
        [Range(0.0f, 1.0f)]
        public float m_LuminanceModulation = 0.8f;

        [Tooltip("Modulates the amount of color bleeding.")]
        [Range(0.0f, 20.0f)]
        public float m_BleedingIntensity = 0f;

        [Range(1, 4)]
        [Tooltip("Amount of downsampling performed when calculating SSAO. Higher is cheaper, but less precise.")]
        public int m_Downsampling = 1;

        [Tooltip("Toggles bilateral blur.")]
        public bool m_Blur = true;

        [Tooltip("Blur will kick in only for samples with less than this difference in depth.")]
        [Range(0,2)]
        public float m_BlurDepthThreshold = 1.0f;

        [Tooltip("Blur will kick in only for samples with less than this difference in normals.")]
        [Range(0,2)]
        public float m_BlurNormalThreshold = 0.1f;

        public VisualizationMode m_Visualization = VisualizationMode.None;

        private Shader m_SSAOShader;
        private Material m_SSAOMaterial;

        private bool m_Supported;

        private static Material CreateMaterial (Shader shader)
        {
            if (!shader)
                return null;
            Material m = new Material (shader);
            m.hideFlags = HideFlags.HideAndDontSave;
            return m;
        }
        private static void DestroyMaterial (Material mat)
        {
            if (mat)
            {
                DestroyImmediate (mat);
                mat = null;
            }
        }

        void OnDisable()
        {
            DestroyMaterial (m_SSAOMaterial);
        }

        void Start()
        {

            if (!SystemInfo.supportsImageEffects || 
                !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth))
            {
                m_Supported = false;
                enabled = false;
                return;
            }

            CreateMaterials ();
            if (!m_SSAOMaterial || m_SSAOMaterial.passCount != 5)
            {
                m_Supported = false;
                enabled = false;
                return;
            }

            m_Supported = true;
        }

        void Awake () {
            cam = GetComponent<Camera>();
        }

        void OnPreCull () {
            cam.depthTextureMode |= DepthTextureMode.DepthNormals;
            cam.depthTextureMode |= DepthTextureMode.Depth;
        }

        private void CreateMaterials ()
        {
            if (!m_SSAOShader)
                m_SSAOShader = Shader.Find("Hidden/Simple SSAO");

            if (!m_SSAOShader){
                Debug.LogError("Could not find required SSSAO shader. Cannot initialize Simple SSAO.");
                return;
            }

            if (!m_SSAOMaterial && m_SSAOShader.isSupported)
            {
                m_SSAOMaterial = CreateMaterial (m_SSAOShader);
            }
        }

        private string[] keywords = new string[2];

        // If this camera is going to render, add SSSAO to it:
        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            
            var act = gameObject.activeInHierarchy && enabled;
            if (!act || !m_Supported)
            {
                Graphics.Blit(source, destination);
                return;
            }

            CreateMaterials ();

            if (m_SSAOMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            // Enable / disable color bleeding:
            if (m_BleedingIntensity > 0)
                keywords[1] = "COLORBLEEDING_ON";

            keywords[0] = (m_Quality == Quality.Low) ? "SAMPLES_2"
                        : (m_Quality == Quality.Medium) ? "SAMPLES_4"
                        : (m_Quality == Quality.High) ? "SAMPLES_6"
                        : (m_Quality == Quality.Ultrahigh) ? "SAMPLES_8" : "SAMPLES_2";

            m_SSAOMaterial.shaderKeywords = keywords;

            // Calculate frustum top right corner, used to reconstruct positions from depth.
            float far = cam.farClipPlane;
            float x,y;
            if (cam.orthographic){
                y = 2 * cam.orthographicSize;
                x = y * cam.aspect;
            }else{
                y = 2 * Mathf.Tan (cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * far;
                x = y * cam.aspect;
            }

            m_SSAOMaterial.SetVector ("_FarCorner", new Vector3(x,y,far));

            m_SSAOMaterial.SetVector ("_Params", new Vector4(
                                                     m_Radius,
                                                     m_OcclusionBias,
                                                     m_OcclusionOffset,
                                                     1.0f/(m_Radius*m_Radius*10)
                                                     ));
            m_SSAOMaterial.SetVector ("_Params2", new Vector4(
                                                     m_OcclusionIntensity,
                                                     m_OcclusionExponent,
                                                     0,
                                                     m_RadiusRange.x));
            m_SSAOMaterial.SetVector ("_Params3", new Vector4(
                                                     m_BleedingIntensity,
                                                     0,
                                                     m_BlurDepthThreshold,
                                                     m_BlurNormalThreshold));
            m_SSAOMaterial.SetVector ("_Params4", new Vector4(m_RadiusRange.y,m_LuminanceModulation,0,0));
            m_SSAOMaterial.SetVector ("_InputSize", new Vector2((int)(cam.pixelWidth*0.5f),
                                                                (int)(cam.pixelHeight*0.5f)));

            m_SSAOMaterial.SetColor("_OcclusionColor",m_OcclusionColor);
            m_SSAOMaterial.SetTexture ("_AxisTexture", m_AxisPattern);

            // prepare ssao target:
            RenderTexture ssao = RenderTexture.GetTemporary(source.width/m_Downsampling, source.height/m_Downsampling, 0, RenderTextureFormat.ARGBHalf);

            int interleavePatternWidth, interleavePaternHeight;
            if (m_AxisPattern) {
                interleavePatternWidth = m_AxisPattern.width;
                interleavePaternHeight = m_AxisPattern.height;
            } else {
                interleavePatternWidth = 1; interleavePaternHeight = 1;
            }
            m_SSAOMaterial.SetVector ("_InterleavePatternScale", new Vector2 ((float)ssao.width / interleavePatternWidth, (float)ssao.height / interleavePaternHeight));

            // prepare downsampled color buffer:
            RenderTexture colorDS = null;
            if (m_BleedingIntensity > 0){
                colorDS = RenderTexture.GetTemporary(ssao.width / 2, ssao.height / 2, 0, source.format);
                colorDS.wrapMode = TextureWrapMode.Clamp;
                colorDS.filterMode = FilterMode.Bilinear;
                Graphics.Blit(source, colorDS);
                m_SSAOMaterial.SetTexture ("_ColorBuffer", colorDS);
            }
    
            // calculate SSAO:
            Graphics.Blit(source, ssao, m_SSAOMaterial, 0);

            if (m_BleedingIntensity > 0)
                RenderTexture.ReleaseTemporary(colorDS);

            if (m_Blur)
            {
                // Blur SSAO horizontally
                RenderTexture rtBlurX = RenderTexture.GetTemporary (source.width, source.height, 0);
                m_SSAOMaterial.SetVector ("_TexelOffsetScale",new Vector4 (1f / ssao.width, 0,0,0));
                m_SSAOMaterial.SetTexture ("_SSAO", ssao);
                Graphics.Blit (null, rtBlurX, m_SSAOMaterial, 1);
                RenderTexture.ReleaseTemporary (ssao); // original rtAO not needed anymore

                // Blur SSAO vertically
                RenderTexture rtBlurY = RenderTexture.GetTemporary (source.width, source.height, 0);
                m_SSAOMaterial.SetVector ("_TexelOffsetScale",new Vector4 (0, 1f / ssao.height, 0,0));
                m_SSAOMaterial.SetTexture ("_SSAO", rtBlurX);
                Graphics.Blit (null, rtBlurY, m_SSAOMaterial, 1);
                RenderTexture.ReleaseTemporary (rtBlurX); // blurX RT not needed anymore

                ssao = rtBlurY; // AO is the blurred one now
            }

            m_SSAOMaterial.SetTexture ("_SSAO", ssao);
            Graphics.Blit(source, destination, m_SSAOMaterial, 2 + (int)m_Visualization);

            RenderTexture.ReleaseTemporary(ssao);
   
        }   
    }
}
