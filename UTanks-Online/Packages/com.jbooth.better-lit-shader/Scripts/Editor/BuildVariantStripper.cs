using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;


namespace JBooth.BetterLit
{
   class BuildVariantStripped : IPreprocessShaders
   {
      List<ShaderKeyword> keywords = new List<ShaderKeyword>();
      BetterLitVariantConfig config;
      public BuildVariantStripped()
      {
         BetterLitVariantConfig c = Resources.Load<BetterLitVariantConfig>("BetterLitVariantConfig");
         if (c == null)
         {
            Debug.Log("Better Lit: No config found in Resources/BetterLitVariantConfig, all possible variants will be included");
            return;
         }
         config = c;
         
         if (c.bakery)
         {
            keywords.Add(new ShaderKeyword("_LIGHTMAPMODE_STANDARD"));
            keywords.Add(new ShaderKeyword("_LIGHTMAPMODE_RNM"));
            keywords.Add(new ShaderKeyword("_LIGHTMAPMODE_SH"));
            keywords.Add(new ShaderKeyword("_LIGHTMAPMODE_VERTEX"));
            keywords.Add(new ShaderKeyword("_LIGHTMAPMODE_VERTEXDIRECTIONAL"));
         }
         if (c.LODCrossfade)
         {
            keywords.Add(new ShaderKeyword("LOD_FADE_CROSSFADE"));
         }

         
      }

      public int callbackOrder { get { return 0; } }

      public void OnProcessShader(
          Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData)
      {
         // In development, don't strip debug variants
         //if (EditorUserBuildSettings.development)
         //    return;

         
         if (shader.name.Contains("Better Lit") && !shader.name.Contains("Development"))
         {
            if (config.deferredVariants)
            {
               if (snippet.passType == PassType.Deferred)
               {
                  Debug.Log(shader + " " + shaderCompilerData.Count);
                  shaderCompilerData.Clear();
               }
            }
            for (int i = 0; i < shaderCompilerData.Count; ++i)
            {
               foreach (var k in keywords)
               {
                  if (shaderCompilerData[i].shaderKeywordSet.IsEnabled(k))
                  {
                     Debug.LogWarning("Removing variant " + shader.name + " for " + k);
                     shaderCompilerData.RemoveAt(i);
                     --i;
                     break;
                  }
               }
            }
         }
      }
   }
}