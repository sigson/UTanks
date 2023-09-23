using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.Services;

namespace SecuredSpace.Battle.Tank
{
    public class ColormapScript : MonoBehaviour
    {
        public bool isStandart;
        public bool isFrameAnimated;
        public bool isVideoClip;
        public bool isShaderAnimation;

        public void ClearFlags()
        {
            isStandart = false;
            isFrameAnimated = false;
            isVideoClip = false;
            isShaderAnimation = false;
        }

        public Texture2D colorTexture;
        public List<Texture2D> colorSlicedTextures;
        public float FrameTimeMs = 0.04f;
        public int currentFrame = 0;
        public VideoClip colorVideo;
        [HideInInspector]
        public CustomRenderTexture renderTexture = null;
        public static Dictionary<CustomRenderTexture, bool> renderStorage = new Dictionary<CustomRenderTexture, bool>();
        public static List<CustomRenderTexture> usingRenderTextures = new List<CustomRenderTexture>();
        public Material shaderMaterial;

        private ConfigObj ColormapSkinConfig;
        private ItemCard ColormapResources;
        private MeshRenderer thisMeshRenderer;
        public bool disabled;
        
        public void DisableColormap() => disabled = true;

        public void Setup(ConfigObj colormapSkinConfig, ItemCard colormapResources, bool force = false)
        {
            if(force)
                ColormapResources = colormapResources;
            if (thisMeshRenderer == null)
                thisMeshRenderer = this.GetComponent<MeshRenderer>();
            if (colormapSkinConfig.GetObject<bool>("shaderAnimation"))
            {
                if (force)
                {
                    ClearFlags();
                    isShaderAnimation = true;
                    //shaderMaterial = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin").GetChildFSObject("card").GetContent<ItemCard>().GetElement<Material>("HullMaterialShaderAnim");
                    if (!colormapResources.ItemData.ContainsKey("image"))
                    {
                        ULogger.Log("error shader colormap - no image");
                        return;
                    }
                    var preRenderTexture = colormapResources.GetElement<CustomRenderTexture>("image");
                    if(renderTexture != null && usingRenderTextures.Contains(renderTexture))
                    {
                        usingRenderTextures.Remove(renderTexture);
                        if (!usingRenderTextures.Contains(renderTexture))
                            renderStorage.Remove(renderTexture);
                    }
                    renderTexture = preRenderTexture;
                    usingRenderTextures.Add(renderTexture);
                    if (!renderStorage.ContainsKey(renderTexture))
                    {
                        renderStorage[renderTexture] = true;
                        renderTexture.Initialize();
                        renderTexture.Update();
                    }
                }
            }
            else if (colormapSkinConfig.Deserialized["frameAnimation"].ToObject<bool>())
            {
                if(force)
                {
                    ClearFlags();
                    isFrameAnimated = true;
                    colorSlicedTextures.Clear();
                    int counter = 0;
                    currentFrame = 0;
                    if (!colormapResources.ItemData.ContainsKey("card"))
                    {
                        ULogger.Log("error frame colormap - no card");
                        return;
                    }
                    var colorCard = colormapResources.GetElement<ItemCard>("card");
                    while (colorCard.ItemData.ContainsKey("image_" + counter.ToString()))
                    {
                        var spritePart = colorCard.GetElement<Sprite>("image_" + counter.ToString());
                        colorSlicedTextures.Add(SecuredSpace.UnityExtend.SpriteExtensions.Duplicate(spritePart, SpriteExtensions.TransformType.None).texture);
                        counter++;
                    }
                    if(colorSlicedTextures.Count > 0)
                    {
                        var hullMaterials = thisMeshRenderer.materials;
                        hullMaterials.ForEach((Material material) => {
                            material.SetTexture("_Colormap", colorSlicedTextures[0]);
                        });
                    }
                }
                
                //var sprite = SecuredSpace.UnityExtend.SpriteExtensions.Duplicate(rawsprite, transformType);
            }
            else if (colormapSkinConfig.Deserialized["videoAnimation"].ToObject<bool>())
            {
                if (force)
                {
                    ClearFlags();
                    //isShaderAnimation = true;
                }
            }
            else
            {
                if (force)
                {
                    ClearFlags();
                    isStandart = true;
                }
            }
            PlayColormap();
        }

        private void OnDestroy()
        {
            if (renderTexture != null && usingRenderTextures.Contains(renderTexture))
            {
                usingRenderTextures.Remove(renderTexture);
                if (!usingRenderTextures.Contains(renderTexture))
                    renderStorage.Remove(renderTexture);
            }
        }

        public void PlayColormap()
        {
            if(isStandart)
            {
                var hullMaterials = thisMeshRenderer.materials;
                hullMaterials.ForEach((Material material) => {
                    material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
                });
            }
            if(isFrameAnimated)
                disabled = false;
            if(isShaderAnimation)
            {
                var hullMaterials = thisMeshRenderer.materials;
                hullMaterials.ForEach((Material material) => {
                    material.SetTexture("_Colormap", renderTexture);
                    //material.SetFloat("_TileSize", shaderMaterial.GetFloat("_TileSize"));
                    //material.SetFloat("_Size", shaderMaterial.GetFloat("_Size"));
                    //material.SetTextureScale("_Colormap", shaderMaterial.GetTextureScale("_Colormap"));
                });
            }
        }

        private float timeCounter = 0;
        public void Update()
        {
            if(isFrameAnimated && !disabled && colorSlicedTextures.Count > 0)
            {
                timeCounter += Time.deltaTime;
                if(timeCounter >= FrameTimeMs)
                {
                    timeCounter = 0;
                    currentFrame = currentFrame + 1 >= colorSlicedTextures.Count - 1 ? currentFrame = 0 : currentFrame + 1;
                    var hullMaterials = this.GetComponent<MeshRenderer>().materials;
                    hullMaterials.ForEach((Material material) => {
                        material.SetTexture("_Colormap", colorSlicedTextures[currentFrame]);
                    });
                }
            }
            if (isShaderAnimation)
            {
                try
                {
                    for( int i = 0; i < renderStorage.Count; i++)
                    {
                        var x = renderStorage.ElementAt(i);
                        if (!x.Value)
                        {
                            x.Key.Initialize();
                            x.Key.Update();
                            renderStorage[x.Key] = true;
                        }
                    }
                }
                catch { }
            }    
        }

        public void LateUpdate()
        {
            try
            {
                for (int i = 0; i < renderStorage.Count; i++)
                    renderStorage[renderStorage.ElementAt(i).Key] = false;
            }
            catch { }
        }
    }
}