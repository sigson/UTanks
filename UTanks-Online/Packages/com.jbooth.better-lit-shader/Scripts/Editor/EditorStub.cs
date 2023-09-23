//#define ALLOWPOM

//////////////////////////////////////////////////////
// Better Lit Shader
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace JBooth.BetterLit
{
   public class LitBaseStub
   {
      ShaderGUI shaderGUI;
      Material mat;
      public LitBaseStub(ShaderGUI sg, Material m)
      {
         shaderGUI = sg;
         mat = m;
         rolloutKeywordStates.Clear();
      }

      public enum ParallaxMode
      {
         Off,
         Parallax,
#if ALLOWPOM
         POM
#endif
      }

      public enum NoiseSpace
      {
         UV,
         Local,
         World
      }

      public enum Packing
      {
         Unity,
         Fastest
      }

      public enum TriplanarSpace
      {
         World,
         Local
      }

      public enum UVMode
      {
         UV,
         Triplanar,
      }

      public enum UVSource
      {
         UV0,
         UV1,
         ProjectX,
         ProjectY,
         ProjectZ
      }

      public enum LayerBlendMode
      {
         Multiply2X,
         AlphaBlended,
         HeightBlended,
      }

      public enum NormalMode
      {
         Textures,
         FromHeight,
         SurfaceGradient
      }

      public enum Workflow
      {
         Metallic,
         Specular
      }



      MaterialProperty FindProperty(string name, MaterialProperty[] props)
      {
         foreach (var p in props)
         {
            if (p != null && p.name == name)
               return p;
         }
         return null;
      }

      public enum TessellationMode
      {
         Edge,
         Distance
      }

      public enum AlphaMode
      {
         Opaque,
         Alpha
      }

      static System.Collections.Generic.Dictionary<string, bool> rolloutStates = new System.Collections.Generic.Dictionary<string, bool>();
      static GUIStyle rolloutStyle;
      public static bool DrawRollup(string text, bool defaultState = true, bool inset = false)
      {
         if (rolloutStyle == null)
         {
            rolloutStyle = new GUIStyle(GUI.skin.box);
            rolloutStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
         var oldColor = GUI.contentColor;
         GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         if (inset == true)
         {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.GetControlRect(GUILayout.Width(40));
         }

         if (!rolloutStates.ContainsKey(text))
         {
            rolloutStates[text] = defaultState;
            string key = text;
            if (EditorPrefs.HasKey(key))
            {
               rolloutStates[text] = EditorPrefs.GetBool(key);
            }
         }
         if (GUILayout.Button(text, rolloutStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) }))
         {
            rolloutStates[text] = !rolloutStates[text];
            string key = text;
            EditorPrefs.SetBool(key, rolloutStates[text]);
         }
         if (inset == true)
         {
            EditorGUILayout.GetControlRect(GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
         }
         GUI.contentColor = oldColor;
         return rolloutStates[text];
      }

      static System.Collections.Generic.Dictionary<string, bool> rolloutKeywordStates = new System.Collections.Generic.Dictionary<string, bool>();

      public static bool DrawRollupKeywordToggle(Material mat, string text, string keyword)
      {
         if (rolloutStyle == null)
         {
            rolloutStyle = new GUIStyle(GUI.skin.box);
            rolloutStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
         var oldColor = GUI.contentColor;

         EditorGUILayout.BeginHorizontal(rolloutStyle);
         bool toggle = mat.IsKeywordEnabled(keyword);
         if (!rolloutKeywordStates.ContainsKey(keyword))
         {
            rolloutKeywordStates[keyword] = toggle;
         }
  
         var nt = EditorGUILayout.Toggle(toggle, GUILayout.Width(18));
         if (nt != toggle)
         {
            mat.DisableKeyword(keyword);
            if (nt)
            {
               mat.EnableKeyword(keyword);
               rolloutKeywordStates[keyword] = true;
            }
            EditorUtility.SetDirty(mat);
         }

         if (GUILayout.Button(text, rolloutStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) }))
         {
            rolloutKeywordStates[keyword] = !rolloutKeywordStates[keyword];
         }
         EditorGUILayout.EndHorizontal();
         GUI.contentColor = oldColor;
         
         return rolloutKeywordStates[keyword];
      }


      public static bool DrawRollupToggle(Material mat, string text, ref bool toggle)
      {
         if (rolloutStyle == null)
         {
            rolloutStyle = new GUIStyle(GUI.skin.box);
            rolloutStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
         var oldColor = GUI.contentColor;

         EditorGUILayout.BeginHorizontal(rolloutStyle);
         if (!rolloutStates.ContainsKey(text))
         {
            rolloutStates[text] = true;
            string key = text;
            if (EditorPrefs.HasKey(key))
            {
               rolloutStates[text] = EditorPrefs.GetBool(key);
            }
         }

         var nt = EditorGUILayout.Toggle(toggle, GUILayout.Width(18));
         if (nt != toggle && nt == true)
         {
            // open when changing toggle state to true
            rolloutStates[text] = true;
            EditorPrefs.SetBool(text, rolloutStates[text]);
         }
         toggle = nt;
         if (GUILayout.Button(text, rolloutStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) }))
         {
            rolloutStates[text] = !rolloutStates[text];
            string key = text;
            EditorPrefs.SetBool(key, rolloutStates[text]);
         }
         EditorGUILayout.EndHorizontal();
         GUI.contentColor = oldColor;
         return rolloutStates[text];
      }

      Texture2D FindDefaultTexture(string name)
      {
         var guids = AssetDatabase.FindAssets("t:Texture2D betterlit_default_");
         foreach (var g in guids)
         {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (path.Contains(name))
            {
               return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
         }
         return null;
      }

      public static void DrawSeparator()
      {
         EditorGUILayout.Separator();
         GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
         EditorGUILayout.Separator();
      }

      public static void WarnLinear(Texture tex)
      {
         if (tex != null)
         {
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
            if (ai != null)
            {
               TextureImporter ti = ai as TextureImporter;
               if (ti != null && ti.sRGBTexture != false)
               {
                  EditorGUILayout.BeginHorizontal();
                  EditorGUILayout.HelpBox("Texture is sRGB! Should be linear!", MessageType.Error);
                  if (GUILayout.Button("Fix"))
                  {
                     ti.sRGBTexture = false;
                     ti.SaveAndReimport();
                  }
                  EditorGUILayout.EndHorizontal();
               }
            }
         }
      }

      public static void WarnNormal(Texture tex)
      {
         if (tex != null)
         {
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
            if (ai != null)
            {
               TextureImporter ti = ai as TextureImporter;
               if (ti != null && ti.textureType != TextureImporterType.NormalMap)
               {
                  EditorGUILayout.BeginHorizontal();
                  EditorGUILayout.HelpBox("Texture is set to type normal!", MessageType.Error);
                  if (GUILayout.Button("Fix"))
                  {
                     ti.textureType = TextureImporterType.NormalMap;
                     ti.SaveAndReimport();
                  }
                  EditorGUILayout.EndHorizontal();
               }
            }
         }
      }

      enum LightingModel
      {
         Standard,
         Simple
      }

      void DoLightingModel()
      {
         LightingModel model = mat.IsKeywordEnabled("_SIMPLELIT") ? LightingModel.Simple : LightingModel.Standard;

         if (model == LightingModel.Simple)
         {
            EditorGUILayout.HelpBox("Simple Lighting only available URP or Built-In forward renderer", MessageType.Info);
         }
         var nm = (LightingModel)EditorGUILayout.EnumPopup("Lighting Model", model);
         if (nm != model)
         {
            if (nm == LightingModel.Simple)
            {
               mat.EnableKeyword("_SIMPLELIT");
            }
            else
            {
               mat.DisableKeyword("_SIMPLELIT");
            }
         }
         
      }

      enum CullMode
      {
         Off = 0,
         Front,
         Back
      }

      enum BackfaceNormalMode
      {
         Flip = 0,
         Mirror = 1,
         None = 2
      }

      public void DoCullMode()
      {
         CullMode cullMode = (CullMode)(int)mat.GetFloat("_CullMode");
         BackfaceNormalMode normalMode = (BackfaceNormalMode)(int)mat.GetFloat("_DoubleSidedNormalMode");

         EditorGUI.BeginChangeCheck();
         cullMode = (CullMode)EditorGUILayout.EnumPopup("Cull Mode", cullMode);
         if (cullMode != CullMode.Back)
         {
            EditorGUI.indentLevel++;
            normalMode = (BackfaceNormalMode)EditorGUILayout.EnumPopup("Back Fade Normal", normalMode);
            EditorGUI.indentLevel--;
         }
         if (EditorGUI.EndChangeCheck())
         {
            mat.SetFloat("_CullMode", (int)cullMode);
            mat.SetFloat("_DoubleSidedNormalMode", (int)normalMode);
            EditorUtility.SetDirty(mat);
         }
      }

      void GetTessellationState(out TessellationMode mode, out bool enabled)
      {
         enabled = mat.HasProperty("_TessellationMaxSubdiv");
         mode = TessellationMode.Distance;
         if (mat.IsKeywordEnabled("_TESSEDGE"))
         {
            mode = TessellationMode.Edge;
         }

      }

      AlphaMode GetAlphaState()
      {
         return mat.HasProperty("_IsAlpha") ? AlphaMode.Alpha : AlphaMode.Opaque;
      }

      void SetChangeShader(AlphaMode alpha, TessellationMode tess, bool tessOn)
      {
         EditorUtility.SetDirty(mat);

         string shaderName = "";
         if (alpha == AlphaMode.Alpha && tessOn)
         {
            shaderName += "Hidden/Better Lit/Lit Tessellation Alpha";
         }
         else if (tessOn)
         {
            shaderName += "Hidden/Better Lit/Lit Tessellation";
         }
         else if (alpha == AlphaMode.Alpha)
         {
            shaderName += "Hidden/Better Lit/Lit Alpha";
         }
         else
         {
            shaderName += "Better Lit/Lit";
         }

         if (tess == TessellationMode.Edge)
         {
            mat.EnableKeyword("_TESSEDGE");
         }
         else
         {
            mat.DisableKeyword("_TESSEDGE");
         }
         Shader s = Shader.Find(shaderName);
         if (s != null && mat.shader != s)
         {
            changeShader = s;
            EditorUtility.SetDirty(mat);
         }
      }

      GUIContent CTessMethod = new GUIContent("Method", "Edge based tessellation tried to keep a consistant edge size, Distance based is based off distance from the camera. Edge can be a little less stable as it's view angle dependent, but is usually a bit more performant");
      // This is only for the final shader editor, not for better shaders.
      public void DoTessellationOption(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         AlphaMode alpha = GetAlphaState();
         TessellationMode tess;
         bool tessOn = false;
         GetTessellationState(out tess, out tessOn);

         EditorGUI.BeginChangeCheck();
         bool show = (DrawRollupToggle(mat, "Tessellation", ref tessOn));
         if (EditorGUI.EndChangeCheck())
         {
            SetChangeShader(alpha, tess, tessOn);
         }
         if (show)
         {
            EditorGUI.BeginChangeCheck();
            tess = (TessellationMode)EditorGUILayout.EnumPopup(CTessMethod, tess);
            if (EditorGUI.EndChangeCheck())
            {
               SetChangeShader(alpha, tess, tessOn);
            }

            if (tessOn)
            {
               OnTessGUI(materialEditor, props);
            }
         }
      }

      // This is only for the final shader editor, not for better shaders.
      public void DoAlphaOptions()
      {
         AlphaMode alpha = GetAlphaState();
         TessellationMode tess;
         bool tessOn = false;
         GetTessellationState(out tess, out tessOn);

         EditorGUI.BeginChangeCheck();

         alpha = (AlphaMode)EditorGUILayout.EnumPopup("Opacity", alpha);


         if (EditorGUI.EndChangeCheck())
         {
            SetChangeShader(alpha, tess, tessOn);
         }
      }

      public Packing GetPacking()
      {
         Packing packing = Packing.Unity;
         if (mat.IsKeywordEnabled("_PACKEDFAST"))
         {
            packing = Packing.Fastest;
         }
         return packing;
      }

      GUIContent CPacking = new GUIContent("Texture Packing", "Unity : PBR Data is packed into 3 textures, Fastest : Packed into 2 textures. See docs for packing format");
      Packing DoPacking(Material mat)
      {
         Packing packing = GetPacking();

         var np = (Packing)EditorGUILayout.EnumPopup(CPacking, packing);

         if (np != packing)
         {
            mat.DisableKeyword("_PACKEDFAST");
            if (np == Packing.Fastest)
            {
               mat.EnableKeyword("_PACKEDFAST");
            }
            EditorUtility.SetDirty(mat);
         }
         return np;
      }

      TriplanarSpace DoTriplanarSpace(Material mat, MaterialEditor materialEditor, MaterialProperty[] props, string spaceProp, string contrastProp)
      {
         EditorGUI.indentLevel++;
         TriplanarSpace space = TriplanarSpace.World;
         if (mat.GetFloat(spaceProp) > 0.5)
            space = TriplanarSpace.Local;

         EditorGUI.BeginChangeCheck();
         space = (TriplanarSpace)EditorGUILayout.EnumPopup("Triplanar Space", space);
         if (EditorGUI.EndChangeCheck())
         {
            mat.SetFloat(spaceProp, (int)space);
            EditorUtility.SetDirty(mat);
         }

         materialEditor.ShaderProperty(FindProperty(contrastProp, props), "Triplanar Contrast");
         EditorGUI.indentLevel--;
         return space;
      }

      

      public void DoFlatShadingMode( MaterialEditor materialEditor, MaterialProperty[] props)
      {
         bool mode = false;
         if (mat.IsKeywordEnabled("_FLATSHADE"))
         {
            mode = true;
         }
         EditorGUILayout.BeginHorizontal();
         EditorGUI.BeginChangeCheck();
         mode = EditorGUILayout.Toggle(mode, GUILayout.Width(24));
         if (EditorGUI.EndChangeCheck())
         {
            if (mode)
            {
               mat.EnableKeyword("_FLATSHADE");
            }
            else
            {
               mat.DisableKeyword("_FLATSHADE");
            }
            EditorUtility.SetDirty(mat);
         }

         var old = GUI.enabled;
         GUI.enabled = mode;

         materialEditor.ShaderProperty(FindProperty("_FlatShadingBlend", props), "Flat Shading");

         EditorGUILayout.EndHorizontal();
         GUI.enabled = old;
      }

      GUIContent CStochastic = new GUIContent("Stochastic", "Prevents visible tiling on surfaces");
      bool DoStochastic(Material mat, MaterialEditor materialEditor, MaterialProperty[] props, string keyword, string prop, string prop2)
      {
         bool mode = false;
         if (mat.IsKeywordEnabled(keyword))
         {
            mode = true;
         }
         EditorGUI.BeginChangeCheck();
         mode = EditorGUILayout.Toggle(CStochastic, mode);
         if (EditorGUI.EndChangeCheck())
         {
            if (mode)
            {
               mat.EnableKeyword(keyword);
            }
            else
            {
               mat.DisableKeyword(keyword);
            }
            EditorUtility.SetDirty(mat);
         }

         var old = GUI.enabled;
         GUI.enabled = mode;
         if (mode)
         {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(FindProperty(prop, props), "Stochastic Contrast");
            materialEditor.ShaderProperty(FindProperty(prop2, props), "Stochastic Scale");
            EditorGUI.indentLevel--;
         }
         GUI.enabled = old;

         return mode;
      }

      public NormalMode GetNormalMode()
      {
         NormalMode normalMode = mat.IsKeywordEnabled("_AUTONORMAL") ? NormalMode.FromHeight : NormalMode.Textures;
         if (mat.IsKeywordEnabled("_SURFACEGRADIENT"))
         {
            normalMode = NormalMode.SurfaceGradient;
         }
         return normalMode;
      }

      GUIContent CNormalMode = new GUIContent("Normal Mode", "Use traditional normal textures, generate them from the height map, or use the surface gradient framework for slightly higher quality normals when blending normals");
      public NormalMode DoNormalMode(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         NormalMode normalMode = GetNormalMode();
         EditorGUI.BeginChangeCheck();
         if (normalMode == NormalMode.FromHeight)
         {
            EditorGUILayout.HelpBox("Albedo must have height values in the alpha channel!", MessageType.Info);
         }

         normalMode = (NormalMode)EditorGUILayout.EnumPopup(CNormalMode, normalMode);
         if (EditorGUI.EndChangeCheck())
         {
            if (normalMode == NormalMode.FromHeight)
            {
               mat.EnableKeyword("_AUTONORMAL");
            }
            else
            {
               mat.DisableKeyword("_AUTONORMAL");
            }
            if (normalMode == NormalMode.SurfaceGradient)
            {
               mat.EnableKeyword("_SURFACEGRADIENT");
            }
            else
            {
               mat.DisableKeyword("_SURFACEGRADIENT");
            }
         }


         if (normalMode == NormalMode.FromHeight)
         {
            materialEditor.ShaderProperty(FindProperty("_AutoNormalStrength", props), "Normal From Height Strength");
         }
         return normalMode;

      }



      UVMode DoUVMode(Material mat, string keyword, string label)
      {
         UVMode uvMode = UVMode.UV;
         if (mat.IsKeywordEnabled(keyword))
         {
            uvMode = UVMode.Triplanar;
         }
         EditorGUI.BeginChangeCheck();
         uvMode = (UVMode)EditorGUILayout.EnumPopup(label, uvMode);
         if (EditorGUI.EndChangeCheck())
         {
            if (uvMode == UVMode.Triplanar)
            {
               mat.EnableKeyword(keyword);
            }
            else
            {
               mat.DisableKeyword(keyword);
            }
            EditorUtility.SetDirty(mat);
         }
         return uvMode;

      }

      NoiseSpace DoNoiseSpace()
      {
         NoiseSpace noiseSpace = NoiseSpace.UV;

         if (mat.IsKeywordEnabled("_NOISELOCAL"))
            noiseSpace = NoiseSpace.Local;
         if (mat.IsKeywordEnabled("_NOISEWORLD"))
            noiseSpace = NoiseSpace.World;


         EditorGUI.BeginChangeCheck();
         noiseSpace = (NoiseSpace)EditorGUILayout.EnumPopup("Noise Space", noiseSpace);

         if (EditorGUI.EndChangeCheck())
         {
            mat.DisableKeyword("_NOISEWORLD");
            mat.DisableKeyword("_NOISELOCAL");
            if (noiseSpace == NoiseSpace.World)
            {
               mat.EnableKeyword("_NOISEWORLD");
            }
            else if (noiseSpace == NoiseSpace.Local)
            {
               mat.EnableKeyword("_NOISELOCAL");
            }
            EditorUtility.SetDirty(mat);
         }
         return noiseSpace;

      }

      enum NoiseQuality
      {
         Texture,
         ProceduralLow,
         ProceduralHigh,
         Worley,

      }

      GUIContent CNoiseQuality = new GUIContent("Noise Quality", "Texture based (fastest), 1 octave of value noise, 3 octaves of value noise, worley noise");
      NoiseQuality DoNoiseQuality(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         NoiseQuality noiseQuality = NoiseQuality.ProceduralLow;
         if (mat.IsKeywordEnabled("_NOISETEXTURE"))
         {
            noiseQuality = NoiseQuality.Texture;
         }
         else if (mat.IsKeywordEnabled("_NOISEHQ"))
         {
            noiseQuality = NoiseQuality.ProceduralHigh;
         }
         else if (mat.IsKeywordEnabled("_NOISEWORLEY"))
         {
            noiseQuality = NoiseQuality.Worley;
         }

         var nq = (NoiseQuality)EditorGUILayout.EnumPopup(CNoiseQuality, noiseQuality);
         if (nq != noiseQuality)
         {
            mat.DisableKeyword("_NOISETEXTURE");
            mat.DisableKeyword("_NOISEHQ");
            mat.DisableKeyword("_NOISEWORLEY");
            if (nq == NoiseQuality.Texture)
            {
               mat.EnableKeyword("_NOISETEXTURE");
            }
            else if (nq == NoiseQuality.ProceduralHigh)
            {
               mat.EnableKeyword("_NOISEHQ");
            }
            else if (nq == NoiseQuality.Worley)
            {
               mat.EnableKeyword("_NOISEWORLEY");
            }
         }

         if (nq == NoiseQuality.Texture)
         {
            var prop = FindProperty("_NoiseTex", props);
            if (prop.textureValue == null)
            {
               prop.textureValue = FindDefaultTexture("betterlit_default_noise");
            }
            materialEditor.TexturePropertySingleLine(new GUIContent("Noise Texture"), prop);
         }
         return nq;
      }



      ParallaxMode DoParallax(Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         ParallaxMode mode = ParallaxMode.Off;
         if (mat.IsKeywordEnabled("_PARALLAX"))
            mode = ParallaxMode.Parallax;
#if ALLOWPOM
         else if (mat.IsKeywordEnabled("_POM"))
            mode = ParallaxMode.POM;
#endif

         EditorGUI.BeginChangeCheck();
         EditorGUILayout.BeginHorizontal();
         mode = (ParallaxMode)EditorGUILayout.EnumPopup("Parallax", mode);
         bool old = GUI.enabled;
         GUI.enabled = mode != ParallaxMode.Off;
         materialEditor.ShaderProperty(FindProperty("_ParallaxHeight", props), "");
         GUI.enabled = old;
         EditorGUILayout.EndHorizontal();
#if ALLOWPOM
      if (mode == ParallaxMode.POM)
      {
         EditorGUI.indentLevel++;
         materialEditor.ShaderProperty(FindProperty("_POMMaxSamples", props), "Max Samples");
         materialEditor.ShaderProperty(FindProperty("_POMMin", props), "Fade Begin");
         materialEditor.ShaderProperty(FindProperty("_POMFade", props), "Fade Range");
         EditorGUI.indentLevel--;
      }
#endif
         if (EditorGUI.EndChangeCheck())
         {
            mat.DisableKeyword("_PARALLAX");
            mat.DisableKeyword("_POM");
            if (mode == ParallaxMode.Parallax)
            {
               mat.EnableKeyword("_PARALLAX");
            }
#if ALLOWPOM
            else if (mode == ParallaxMode.POM)
            {
               mat.EnableKeyword("_POM");
            }
#endif
            EditorUtility.SetDirty(mat);
         }
         return mode;
      }

      Workflow GetWorkflow()
      {
         Workflow workflow = Workflow.Metallic;
         if (mat.IsKeywordEnabled("_SPECULAR"))
         {
            workflow = Workflow.Specular;
         }
         return workflow;
      }

      Workflow DoWorkflow()
      {
         Workflow workflow = GetWorkflow();

         var nw = (Workflow)EditorGUILayout.EnumPopup("Workflow", workflow);
         if (nw != workflow)
         {
            if (nw == Workflow.Metallic)
            {
               mat.DisableKeyword("_SPECULAR");
            }
            else
            {
               mat.EnableKeyword("_SPECULAR");
            }
         }
         return nw;
      }

      UVSource DoUVSource(Material mat, string propName)
      {
         UVSource s = (UVSource)((int)mat.GetFloat(propName));
         EditorGUI.BeginChangeCheck();
         s = (UVSource)EditorGUILayout.EnumPopup("UV Source", s);
         if (EditorGUI.EndChangeCheck())
         {
            mat.SetFloat(propName, (float)s);
         }
         return s;
      }

      enum MaskMode
      {
         Vertex,
         Texture
      }

      GUIContent CTextureLayerWeights = new GUIContent("Texture Layer Weights", "Do we weight the texture layers with the vertex colors or with a texture?");
      public void DoTextureLayerWeights(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         bool on = mat.IsKeywordEnabled("_LAYERVERTEXMASK") || mat.IsKeywordEnabled("_LAYERTEXTUREMASK");
         EditorGUI.BeginChangeCheck();
         bool show = (DrawRollupToggle(mat, "Texture Layer Weights", ref on));
         if (EditorGUI.EndChangeCheck())
         {
            if (!on)
            {
               mat.DisableKeyword("_LAYERVERTEXMASK");
               mat.DisableKeyword("_LAYERTEXTUREMASK");
            }
            else if (on)
            {
               if (!mat.IsKeywordEnabled("_LAYERVERTEXMASK") && !mat.IsKeywordEnabled("_LAYERTEXTUREMASK"))
               {
                  mat.EnableKeyword("_LAYERVERTEXMASK");
               }
            }
         }

         if (show)
         {
            var old = GUI.enabled;
            GUI.enabled = on;
            MaskMode mask = MaskMode.Vertex;
            if (mat.IsKeywordEnabled("_LAYERVERTEXMASK"))
            {
               mask = MaskMode.Vertex;
            }
            else if (mat.IsKeywordEnabled("_LAYERTEXTUREMASK"))
            {
               mask = MaskMode.Texture;
            }

            EditorGUI.BeginChangeCheck();
            mask = (MaskMode)EditorGUILayout.EnumPopup(CTextureLayerWeights, mask);

            if (EditorGUI.EndChangeCheck())
            {
               mat.DisableKeyword("_LAYERVERTEXMASK");
               mat.DisableKeyword("_LAYERTEXTUREMASK");
               if (on)
               {
                  if (mask == MaskMode.Vertex)
                  {
                     mat.EnableKeyword("_LAYERVERTEXMASK");
                  }
                  else if (mask == MaskMode.Texture)
                  {
                     mat.EnableKeyword("_LAYERTEXTUREMASK");
                  }
               }
            }
            if (on && mask == MaskMode.Texture)
            {
               EditorGUI.indentLevel++;
               DoUVSource(mat, "_LayerTextureMaskUVMode");
               materialEditor.TexturePropertySingleLine(new GUIContent("Texture Mask"), FindProperty("_LayerTextureMask", props));
               materialEditor.TextureScaleOffsetProperty(FindProperty("_LayerTextureMask", props));
               EditorGUI.indentLevel--;
            }
            GUI.enabled = old;
         }
      }

      public void DoTintMask(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_TintMask"))
            return;
         if (DrawRollupKeywordToggle(mat, "Tint Mask", "_TINTMASK"))
         {
            materialEditor.TexturePropertySingleLine(new GUIContent("Tint Mask"), FindProperty("_TintMask", props));
            materialEditor.TextureScaleOffsetProperty(FindProperty("_TintMask", props));
            materialEditor.ColorProperty(FindProperty("_RColor", props), "R Color");
            materialEditor.ColorProperty(FindProperty("_GColor", props), "G Color");
            materialEditor.ColorProperty(FindProperty("_BColor", props), "B Color");
            materialEditor.ColorProperty(FindProperty("_AColor", props), "A Color");
         }
      }

      public void DoDissolve(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_DissolveTexture"))
            return;
         if (DrawRollupKeywordToggle(mat, "Dissolve", "_DISSOLVE"))
         {
            materialEditor.TexturePropertySingleLine(new GUIContent("Texture"), FindProperty("_DissolveTexture", props));
            materialEditor.TextureScaleOffsetProperty(FindProperty("_DissolveTexture", props));
            materialEditor.TexturePropertySingleLine(new GUIContent("Gradient"), FindProperty("_DissolveGradient", props));
            materialEditor.RangeProperty(FindProperty("_DissolveAmount", props), "Amount");
            materialEditor.RangeProperty(FindProperty("_DissolveColoration", props), "Colorization");
            materialEditor.RangeProperty(FindProperty("_DissolveEdgeContrast", props), "Contrast");
            materialEditor.RangeProperty(FindProperty("_DissolveEmissiveStr", props), "Emissive Strength");
         }
      }

      GUIContent CTraxPackedNormal = new GUIContent("Packed Map", "Normal in fastest packed format, see docs for details");
      public void DoTrax(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (DrawRollupKeywordToggle(mat, "Trax", "_TRAX_ON"))
         {
            materialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), FindProperty("_TraxAlbedo", props), FindProperty("_TraxTint", props));
            materialEditor.TextureScaleOffsetProperty(FindProperty("_TraxAlbedo", props));
            materialEditor.TexturePropertySingleLine(CTraxPackedNormal, FindProperty("_TraxPackedNormal", props), FindProperty("_TraxNormalStrength", props));
            materialEditor.RangeProperty(FindProperty("_TraxInterpContrast", props), "Interpolation Contrast");
            materialEditor.RangeProperty(FindProperty("_TraxHeightContrast", props), "Height Blend Contrast");
            if (mat.HasProperty("_TessellationMaxSubdiv"))
            {
               materialEditor.FloatProperty(FindProperty("_TraxDisplacementDepth", props), "Trax Depression Depth");
               materialEditor.RangeProperty(FindProperty("_TraxDisplacementStrength", props), "Trax Displacement Strength");
               materialEditor.RangeProperty(FindProperty("_TraxMipBias", props), "Trax Mip Bias");
            }
         }
      }

      public void DoBakery(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (DrawRollupKeywordToggle(mat, "Bakery", "USEBAKERY"))
         {
            materialEditor.ShaderProperty(FindProperty("_LightmapMode", props), "LightMap Mode");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_VERTEXLMMASK", props), "Enable vertex shadowmask");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_SHNONLINEAR", props), "SH non-linear mode");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_LMSPEC", props), "Enable Lightmap Specular");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_BICUBIC", props), "Enable Bicubic Filter");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_VOLUME", props), "Use volumes");
            materialEditor.ShaderProperty(FindProperty("_BAKERY_VOLROTATION", props), "Allow volume rotation");
         }
      }

      enum WindUVSpace
      {
         World,
         UV
      }

      public void DoWind(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_WindParticulateSpace"))
            return;
         if (DrawRollupKeywordToggle(mat, "Wind", "_WIND_ON"))
         {
            WindUVSpace space = (WindUVSpace)(int)mat.GetFloat("_WindParticulateSpace");
            EditorGUI.BeginChangeCheck();
            space = (WindUVSpace)EditorGUILayout.EnumPopup(new GUIContent("UV Space"), space);
            if (EditorGUI.EndChangeCheck())
            {
               FindProperty("_WindParticulateSpace", props).floatValue = (int)space;
            }
            var tex = FindProperty("_WindParticulate", props);
            if (mat.IsKeywordEnabled("_WIND_ON"))
            {
               if (tex.textureValue == null)
               {
                  tex.textureValue = FindDefaultTexture("betterlit_default_wind");
                  mat.SetTextureScale("_WindParticulate", new Vector2(0.05f, 0.2f));
               }
               WarnLinear(tex.textureValue);
            }
            materialEditor.TexturePropertySingleLine(new GUIContent("Wind Texture"), tex);
            Vector2 scale = mat.GetTextureScale("_WindParticulate");
            EditorGUI.BeginChangeCheck();
            scale.x = EditorGUILayout.FloatField("Length", scale.x);
            scale.y = EditorGUILayout.FloatField("Width", scale.y);
            if (EditorGUI.EndChangeCheck())
            {
               mat.SetTextureScale("_WindParticulate", scale);
            }
            materialEditor.RangeProperty(FindProperty("_WindParticulateStrength", props), "Strength");
            materialEditor.FloatProperty(FindProperty("_WindParticulateSpeed", props), "Speed");
            materialEditor.RangeProperty(FindProperty("_WindParticulatePower", props), "Power");
            materialEditor.FloatProperty(FindProperty("_WindParticulateRotation", props), "Rotation");
            materialEditor.ColorProperty(FindProperty("_WindParticulateColor", props), "Color");
            materialEditor.VectorProperty(FindProperty("_WindParticulateWorldHeightMask", props), "World Height Mask");
            materialEditor.RangeProperty(FindProperty("_WindParticulateTextureHeight", props), "Heightfield Height");
            materialEditor.VectorProperty(FindProperty("_WindParticulateAngleMask", props), "Angle Mask");
            materialEditor.RangeProperty(FindProperty("_WindParticulateOcclusionStrength", props), "Occlusion Strength");
            materialEditor.ColorProperty(FindProperty("_WindParticulateEmissive", props), "Emissive");
         }
      }

      public void DoTextureLayer(MaterialEditor materialEditor, MaterialProperty[] props, Packing packing, int index)
      {
         string ext = "";
         string def = "";
         if (index > 0)
         {
            def = "_DEF_" + index;
            ext = "_Ext_" + index;
         }
         if (!mat.HasProperty("_LayerTriplanarSpace" + ext))
            return;

         bool rollup = DrawRollupKeywordToggle(mat, "Texture Layer " + index, "_USELAYER" + def);

         if (rollup)
         {
            EditorGUI.indentLevel++;
            UVMode layerUVMode = DoUVMode(mat, "_LAYERTRIPLANAR" + def, "Layer UV Mode");
            if (layerUVMode == UVMode.Triplanar)
            {
               DoTriplanarSpace(mat, materialEditor, props, "_LayerTriplanarSpace" + ext, "_LayerTriplanarContrast" + ext);
            }
            else
            {
               var uvSpace = DoUVSource(mat, "_LayerUVSource" + ext);
               if (uvSpace != UVSource.UV0 && uvSpace != UVSource.UV1)
               {
                  EditorGUI.BeginChangeCheck();
                  TriplanarSpace space = TriplanarSpace.World;
                  if (mat.GetFloat("_LayerTriplanarSpace" + ext) > 0.5)
                     space = TriplanarSpace.Local;
                  space = (TriplanarSpace)EditorGUILayout.EnumPopup("Projection Space", space);
                  if (EditorGUI.EndChangeCheck())
                  {
                     mat.SetFloat("_LayerTriplanarSpace" + ext, (int)space);
                     EditorUtility.SetDirty(mat);
                  }
               }
            }
            
            bool noiseOn = false;
            LayerBlendMode blendMode = (LayerBlendMode)(int)mat.GetFloat("_LayerBlendMode" + ext);

            if (mat.IsKeywordEnabled("_LAYERNOISE" + def))
               noiseOn = true;


            EditorGUI.BeginChangeCheck();
            blendMode = (LayerBlendMode)EditorGUILayout.EnumPopup("Layer Blend Mode", blendMode);
            if (blendMode == LayerBlendMode.HeightBlended)
            {
               materialEditor.ShaderProperty(FindProperty("_LayerStrength" + ext, props), "Layer Height");
               materialEditor.ShaderProperty(FindProperty("_LayerHeightContrast" + ext, props), "Blend Contrast");
            }
            else
            {
               materialEditor.ShaderProperty(FindProperty("_LayerStrength" + ext, props), "Layer Strength");
            }
            noiseOn = EditorGUILayout.Toggle("Layer Noise", noiseOn);

            if (EditorGUI.EndChangeCheck())
            {
               mat.DisableKeyword("_LAYERHEIGHTBLEND" + def);
               mat.DisableKeyword("_LAYERALPHABLEND" + def);
               mat.DisableKeyword("_LAYERNOISE" + def);

               mat.SetFloat("_LayerBlendMode" + ext, (int)blendMode);

               if (noiseOn == true)
               {
                  mat.EnableKeyword("_LAYERNOISE" + def);
               }
               EditorUtility.SetDirty(mat);
            }


            if (noiseOn)
            {
               EditorGUI.indentLevel++;
               EditorGUI.BeginChangeCheck();

               materialEditor.ShaderProperty(FindProperty("_LayerNoiseFrequency" + ext, props), "Noise Frequency");
               materialEditor.ShaderProperty(FindProperty("_LayerNoiseAmplitude" + ext, props), "Noise Amplitude");
               materialEditor.ShaderProperty(FindProperty("_LayerNoiseCenter" + ext, props), "Noise Center");
               materialEditor.ShaderProperty(FindProperty("_LayerNoiseOffset" + ext, props), "Noise Offset");
               EditorGUILayout.BeginHorizontal();
               materialEditor.ShaderProperty(FindProperty("_LayerBlendTint" + ext, props), "Layer Blend Tint");
               materialEditor.ShaderProperty(FindProperty("_LayerBlendContrast" + ext, props), "");
               EditorGUILayout.EndHorizontal();
               if (EditorGUI.EndChangeCheck())
               {
                  EditorUtility.SetDirty(mat);
               }

               EditorGUI.indentLevel--;
            }

            bool angleFilter = mat.IsKeywordEnabled("_LAYERANGLEFILTER" + def);
            bool naf = EditorGUILayout.Toggle("Angle/Height Filter", angleFilter);
            if (naf != angleFilter)
            {
               mat.DisableKeyword("_LAYERANGLEFILTER" + def);
               if (naf)
               {
                  mat.EnableKeyword("_LAYERANGLEFILTER" + def);
               }
            }
            if (naf)
            {
               EditorGUI.indentLevel++;
               materialEditor.ShaderProperty(FindProperty("_LayerAngleMin" + ext, props), "Angle Minimum");
               materialEditor.ShaderProperty(FindProperty("_LayerVertexNormalBlend" + ext, props), "Vertex -> Normal Filter");
               materialEditor.ShaderProperty(FindProperty("_LayerHeight" + ext, props), "Height Filter");
               materialEditor.ShaderProperty(FindProperty("_LayerInvertHeight" + ext, props), "Layer on");
               materialEditor.ShaderProperty(FindProperty("_LayerFalloff" + ext, props), "Contrast");

               EditorGUI.indentLevel--;

            }
            Vector4 dist = mat.GetVector("_LayerWeightOverDistance" + ext);
            if (DrawRollup("Distance Fade", true, true))
            {
               EditorGUI.indentLevel++;
               EditorGUI.BeginChangeCheck();
               dist.x = EditorGUILayout.FloatField("Start Fade", dist.x);
               dist.y = EditorGUILayout.Slider("Start Weight", dist.y, 0, 1);
               dist.z = EditorGUILayout.FloatField("Fade Range", dist.z);
               dist.w = EditorGUILayout.Slider("End Weight", dist.w, 0, 1);
               EditorGUI.indentLevel--;
            
               if (EditorGUI.EndChangeCheck())
               {
                  FindProperty("_LayerWeightOverDistance" + ext, props).vectorValue = dist;
               }
            }

            EditorGUI.BeginChangeCheck();
            var detailAlbedo = FindProperty("_LayerAlbedoMap" + ext, props);
            var detailNormal = FindProperty("_LayerNormalMap" + ext, props);
            var detailMask = FindProperty("_LayerMaskMap" + ext, props);
            var detailEmission = FindProperty("_LayerEmissionMap" + ext, props);
            var detailEmissionColor = FindProperty("_LayerEmissionColor" + ext, props);
            
            materialEditor.TexturePropertySingleLine(new GUIContent("Layer Albedo"), detailAlbedo, FindProperty("_LayerTint" + ext, props));
            materialEditor.TextureScaleOffsetProperty(FindProperty("_LayerAlbedoMap" + ext, props));
            EditorGUI.indentLevel++;
            materialEditor.RangeProperty(FindProperty("_LayerAlbedoBrightness" + ext, props), "Brightness");
            materialEditor.RangeProperty(FindProperty("_LayerAlbedoContrast" + ext, props), "Contrast");
            EditorGUI.indentLevel--;
            if (!mat.IsKeywordEnabled("_AUTONORMAL"))
            {
               if (packing == Packing.Fastest)
               {
                  WarnLinear(detailNormal.textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Layer Packed"), detailNormal);
                  materialEditor.RangeProperty(FindProperty("_LayerNormalStrength" + ext, props), "Normal Strength");
               }
               else
               {
                  WarnNormal(detailNormal.textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Layer Normal"), detailNormal, FindProperty("_LayerNormalStrength" + ext, props));
               }
            }
            if (packing == Packing.Unity)
            {
               WarnLinear(detailMask.textureValue);
               materialEditor.TexturePropertySingleLine(new GUIContent("Layer Mask"), detailMask);
            }
            GUILayout.BeginHorizontal();
            
            bool emissionEnabled = mat.IsKeywordEnabled("_LAYEREMISSION" + def);
            emissionEnabled = EditorGUILayout.Toggle(emissionEnabled, GUILayout.Width(24));
            materialEditor.TexturePropertyWithHDRColor(new GUIContent("Layer Emission"), detailEmission, detailEmissionColor, false);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
               mat.DisableKeyword("_LAYERALBEDO" + def);
               mat.DisableKeyword("_LAYERNORMAL" + def);
               mat.DisableKeyword("_LAYERMASK" + def);
               mat.DisableKeyword("_LAYEREMISSION" + def);

               if (detailAlbedo.textureValue != null)
               {
                  mat.EnableKeyword("_LAYERALBEDO" + def);
               }
               if (detailNormal.textureValue != null)
               {
                  mat.EnableKeyword("_LAYERNORMAL" + def);
               }
               if (detailMask.textureValue != null)
               {
                  mat.EnableKeyword("_LAYERMASK" + def);
               }
               if (emissionEnabled)
               {
                  mat.EnableKeyword("_LAYEREMISSION" + def);
               }
            }

            if (detailMask.textureValue == null && packing != Packing.Fastest)
            {
               materialEditor.ShaderProperty(FindProperty("_LayerSmoothness" + ext, props), new GUIContent("Layer Smoothness"));
               materialEditor.ShaderProperty(FindProperty("_LayerMetallic" + ext, props), new GUIContent("Layer Metallic"));
            }
            else if (packing == Packing.Fastest)
            {
               materialEditor.ShaderProperty(FindProperty("_LayerMetallic" + ext, props), new GUIContent("Layer Metallic"));
               RemapRange("Smoothness Remap", "_LayerSmoothnessRemap" + ext);
               RemapRange("Occlusion Remap", "_LayerAORemap" + ext);
            }
            else if (packing == Packing.Unity && detailMask.textureValue != null)
            {
               RemapRange("Smoothness Remap", "_LayerSmoothnessRemap" + ext);
               RemapRange("Metallic Remap", "_LayerMetallicRemap" + ext);
               RemapRange("Occlusion Remap", "_LayerAORemap" + ext);
            }
            if (mat.HasProperty("_IsAlpha"))
            {
               RemapRange("Alpha Remap", "_LayerHeightRemap" + ext);
            }
            else
            {
               RemapRange("Height Remap", "_LayerHeightRemap" + ext);
            }

            DoStochastic(mat, materialEditor, props, "_LAYERSTOCHASTIC" + def, "_LayerStochasticContrast" + ext, "_LayerStochasticScale" + ext);


            if (blendMode != LayerBlendMode.HeightBlended)
            {
               if (detailAlbedo.textureValue != null)
               {
                  materialEditor.ShaderProperty(FindProperty("_LayerAlbedoStrength" + ext, props), new GUIContent("Layer Albedo Strength"));
               }
               if (detailMask.textureValue != null || packing == Packing.Fastest)
               {
                  materialEditor.ShaderProperty(FindProperty("_LayerSmoothnessStrength" + ext, props), new GUIContent("Layer Smoothness Strength"));
               }
            }
            materialEditor.RangeProperty(FindProperty("_LayerMicroShadowStrength" + ext, props), "Micro Shadow Strength");
            if (mat.shader != null && mat.HasProperty("_TessellationMaxSubdiv"))
            {
               materialEditor.ShaderProperty(FindProperty("_LayerTessStrength" + ext, props), "Displacement Strength");
            }
            DoFuzzyShadingUI(materialEditor, props, "Layer", ext);
            EditorGUI.indentLevel--;
         }

      }

      enum RainMode
      {
         Off,
         Local,
         Global
      }

      public void DoRainDrops(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         RainMode mode = (RainMode)mat.GetInt("_RainMode");

         var nm = (RainMode)EditorGUILayout.EnumPopup("Rain Drops", mode);

         if (nm != mode)
         {
            mode = nm;
            mat.DisableKeyword("_RAINDROPS");
            mat.SetFloat("_RainMode", 0);
            if (mode == RainMode.Local)
            {
               mat.EnableKeyword("_RAINDROPS");
               mat.SetFloat("_RainMode", 1);
            }
            else if (mode == RainMode.Global)
            {
               mat.EnableKeyword("_RAINDROPS");
               mat.SetFloat("_RainMode", 2);
            }
            EditorUtility.SetDirty(mat);
         }
         if (mode != RainMode.Off)
         {
            EditorGUI.indentLevel++;
            var prop = FindProperty("_RainDropTexture", props);
            if (prop.textureValue == null)
            {
               prop.textureValue = FindDefaultTexture("betterlit_default_raindrops");
            }
            materialEditor.TexturePropertySingleLine(new GUIContent("Rain Texture"), FindProperty("_RainDropTexture", props));
            Vector4 data = mat.GetVector("_RainIntensityScale");
            EditorGUI.BeginChangeCheck();
            if (mode != RainMode.Global)
            {
               data.x = EditorGUILayout.Slider("Intensity", data.x, 0, 2);
            }
            data.y = EditorGUILayout.FloatField("UV Scale", data.y);
            data.z = EditorGUILayout.Slider("Effect Wet Areas", data.z, 0, 1);
            float oldW = data.w;
            data.w = EditorGUILayout.FloatField("Distance Falloff", data.w);
            // revision
            if (oldW == data.w && data.w == 0)
            {
               data.w = 200;
               mat.SetVector("_RainIntensityScale", data);
               EditorUtility.SetDirty(mat);
            }
            if (EditorGUI.EndChangeCheck())
            {
               mat.SetVector("_RainIntensityScale", data);
               EditorUtility.SetDirty(mat);
            }
            EditorGUI.indentLevel--;
         }

      }

      public void DoPuddles(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_PuddleMode"))
            return;
         if (DrawRollupKeywordToggle(mat, "Puddles", "_PUDDLES"))
         {
            LocalGlobalMode mode = (LocalGlobalMode)mat.GetInt("_PuddleMode");

            EditorGUI.BeginChangeCheck();
            mode = (LocalGlobalMode)EditorGUILayout.EnumPopup("Puddle Mode", mode);

            if (EditorGUI.EndChangeCheck())
            {
               mat.SetFloat("_PuddleMode", (int)mode);
            }
            EditorGUI.indentLevel++;
            if (mode == LocalGlobalMode.Local)
            {
               materialEditor.ShaderProperty(FindProperty("_PuddleAmount", props), "Puddle Amount");
            }

            materialEditor.ShaderProperty(FindProperty("_PuddleColor", props), "Puddle Color");
            materialEditor.ShaderProperty(FindProperty("_PuddleAngleMin", props), "Puddle Angle Filter");
            materialEditor.ShaderProperty(FindProperty("_PuddleFalloff", props), "Puddle Contrast");
            

            bool noiseOn = mat.IsKeywordEnabled("_PUDDLENOISE");


            EditorGUI.BeginChangeCheck();
            noiseOn = EditorGUILayout.Toggle("Puddle Noise", noiseOn);

            if (EditorGUI.EndChangeCheck())
            {
               mat.DisableKeyword("_PUDDLENOISE");
               if (noiseOn)
               {
                  mat.EnableKeyword("_PUDDLENOISE");
               }
               EditorUtility.SetDirty(mat);
            }

            if (noiseOn)
            {
               EditorGUI.indentLevel++;
               EditorGUI.BeginChangeCheck();

               materialEditor.ShaderProperty(FindProperty("_PuddleNoiseFrequency", props), "Noise Frequency");
               materialEditor.ShaderProperty(FindProperty("_PuddleNoiseAmplitude", props), "Noise Amplitude");
               materialEditor.ShaderProperty(FindProperty("_PuddleNoiseCenter", props), "Noise Center");
               materialEditor.ShaderProperty(FindProperty("_PuddleNoiseOffset", props), "Noise Offset");

               if (EditorGUI.EndChangeCheck())
               {
                  EditorUtility.SetDirty(mat);
               }
               EditorGUI.indentLevel--;
            }

            DoRainDrops(materialEditor, props);
            EditorGUI.indentLevel--;
         }
         

      }

      void RemapRange(string label, string prop, float min = 0, float max = 1)
      {
         Vector4 value = mat.GetVector(prop);
         float low = value.x;
         float high = value.y;
         EditorGUILayout.MinMaxSlider(label, ref low, ref high, min, max);
         if (low != value.x || high != value.y)
         {
            mat.SetVector(prop, new Vector2(low, high));
            EditorUtility.SetDirty(mat);
         }
      }

      public void OnLitShaderSettings(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         Workflow workflow = DoWorkflow();
         DoLightingModel();

         var packing = DoPacking(mat);

         
         DoNoiseSpace();
         DoNoiseQuality(materialEditor, props);
         
      }

      void DoFuzzyShadingUI(MaterialEditor materialEditor, MaterialProperty[] props, string prefix, string postfix)
      {
         bool on = mat.GetFloat("_" + prefix + "FuzzyShadingOn" + postfix) > 0;
         bool change = EditorGUILayout.Toggle("Fuzzy Shading", on);
         if (on != change)
         {
            mat.SetFloat("_" + prefix + "FuzzyShadingOn" + postfix, change ? 1 : 0);
            EditorUtility.SetDirty(mat);
            on = change;
         }
         if (on)
         {
            EditorGUI.indentLevel++;
            materialEditor.ColorProperty(FindProperty("_" + prefix + "FuzzyShadingColor" + postfix, props), "Color");
            Vector4 param = mat.GetVector("_" + prefix + "FuzzyShadingParams" + postfix);
            EditorGUI.BeginChangeCheck();
            param.x = EditorGUILayout.Slider("Core Multiplier", param.x, 0.1f, 3);
            param.y = EditorGUILayout.Slider("Edge Multiplier", param.y, 0.1f, 3);
            param.z = EditorGUILayout.Slider("Power", param.z, 0.1f, 3);
            if (EditorGUI.EndChangeCheck())
            {
               mat.SetVector("_" + prefix + "FuzzyShadingParams" + postfix, param);
            }
            EditorGUI.indentLevel--;
         }

      }

      public Shader changeShader = null;
      public void OnLitGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (DrawRollup("Main Layer"))
         {
            UVMode uvMode = DoUVMode(mat, "_TRIPLANAR", "UV Mode");
            if (uvMode == UVMode.Triplanar)
            {
               DoTriplanarSpace(mat, materialEditor, props, "_TriplanarSpace", "_TriplanarContrast");
            }
            if (uvMode == UVMode.UV)
            {
               var uvSpace = DoUVSource(mat, "_UVSource");
               
               if (uvSpace != UVSource.UV0 && uvSpace != UVSource.UV1)
               {
                  EditorGUI.BeginChangeCheck();
                  TriplanarSpace space = TriplanarSpace.World;
                  if (mat.GetFloat("_TriplanarSpace") > 0.5)
                     space = TriplanarSpace.Local;
                  space = (TriplanarSpace)EditorGUILayout.EnumPopup("Projection Space", space);
                  if (EditorGUI.EndChangeCheck())
                  {
                     mat.SetFloat("_TriplanarSpace", (int)space);
                     EditorUtility.SetDirty(mat);
                  }
               }
            }

            if (mat.GetFloat("_IsConverted") < 1)
            {
               if (mat.GetTexture("_AlbedoMap") == null)
               {
                  var tex = (mat.GetTexture("_MainTex"));
                  if (tex == null)
                     tex = mat.GetTexture("_BaseColor");
                  if (tex != null)
                  {
                     mat.SetTexture("_AlbedoMap", tex);
                  }

               }

               if (mat.GetTexture("_NormalMap") == null)
               {
                  var tex = (mat.GetTexture("_BumpMap"));
                  if (tex != null)
                     mat.SetTexture("_NormalMap", tex);
               }
               mat.SetFloat("_IsConverted", 1);
               if (mat.GetTexture("_NormalMap"))
               {
                  mat.EnableKeyword("_NORMALMAP");
               }
               if (mat.GetTexture("_MaskMap"))
               {
                  mat.EnableKeyword("_MASKMAP");
               }
            }
            
            materialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), FindProperty("_AlbedoMap", props), FindProperty("_Tint", props));
            
            materialEditor.TextureScaleOffsetProperty(FindProperty("_AlbedoMap", props));
            EditorGUI.indentLevel++;
            materialEditor.RangeProperty(FindProperty("_AlbedoBrightness", props), "Brightness");
            materialEditor.RangeProperty(FindProperty("_AlbedoContrast", props), "Contrast");
            EditorGUI.indentLevel--;

            DoParallax(mat, materialEditor, props);

            var normalMode = GetNormalMode();
            var packing = GetPacking();

            if (normalMode != NormalMode.FromHeight)
            {
               EditorGUI.BeginChangeCheck();
               if (packing == Packing.Unity)
               {
                  WarnNormal(FindProperty("_NormalMap", props).textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Normal"), FindProperty("_NormalMap", props), FindProperty("_NormalStrength", props));
               }
               else
               {
                  WarnLinear(FindProperty("_NormalMap", props).textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Packed"), FindProperty("_NormalMap", props), FindProperty("_NormalStrength", props));
               }
               if (EditorGUI.EndChangeCheck())
               {
                  if (FindProperty("_NormalMap", props).textureValue != null)
                  {
                     mat.EnableKeyword("_NORMALMAP");
                  }
                  else
                  {
                     mat.DisableKeyword("_NORMALMAP");
                  }
                  EditorUtility.SetDirty(mat);
               }
            }
            var maskProp = FindProperty("_MaskMap", props);
            if (packing == Packing.Unity)
            {
               EditorGUI.BeginChangeCheck();
               WarnLinear(maskProp.textureValue);
               materialEditor.TexturePropertySingleLine(new GUIContent("Mask Map"), maskProp);
               if (EditorGUI.EndChangeCheck())
               {
                  if (maskProp.textureValue != null)
                  {
                     mat.EnableKeyword("_MASKMAP");
                  }
                  else
                  {
                     mat.DisableKeyword("_MASKMAP");
                  }
               }
            }
            else
            {
               if (mat.IsKeywordEnabled("_MASKMAP"))
               {
                  mat.DisableKeyword("_MASKMAP");
                  EditorUtility.SetDirty(mat);
               }
            }
            var workflow = GetWorkflow();
            if (workflow == Workflow.Specular)
            {
               materialEditor.TexturePropertySingleLine(new GUIContent("Specular Map"), FindProperty("_SpecularMap", props));
            }

            var emissionProp = FindProperty("_EmissionMap", props);
            var emissionColor = FindProperty("_EmissionColor", props);
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            bool emissionEnabled = mat.IsKeywordEnabled("_EMISSION");
            emissionEnabled = EditorGUILayout.Toggle(emissionEnabled, GUILayout.Width(18));
            materialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission"), emissionProp, emissionColor, false);
            GUILayout.EndHorizontal();
            materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
            if (EditorGUI.EndChangeCheck())
            {
               if (emissionEnabled)
               {
                  mat.EnableKeyword("_EMISSION");
               }
               else
               {
                  mat.DisableKeyword("_EMISSION");
               }
            }

            if (packing == Packing.Unity && maskProp.textureValue == null)
            {
               materialEditor.ShaderProperty(FindProperty("_Smoothness", props), "Smoothness");
               materialEditor.ShaderProperty(FindProperty("_Metallic", props), "Metallic");
            }
            else if (packing != Packing.Unity)
            {
               materialEditor.ShaderProperty(FindProperty("_Metallic", props), "Metallic");
               RemapRange("Smoothness Remap", "_SmoothnessRemap");
               RemapRange("Occlusion Remap", "_AORemap");
            }
            if (packing == Packing.Unity && maskProp.textureValue != null)
            {
               RemapRange("Smoothness Remap", "_SmoothnessRemap");
               RemapRange("Metallic Remap", "_MetallicRemap");
               RemapRange("Occlusion Remap", "_AORemap");
            }

            if (mat.HasProperty("_IsAlpha"))
            {
               RemapRange("Alpha Remap", "_HeightRemap");
            }
            else
            {
               RemapRange("Height Remap", "_HeightRemap");
            }
            if (mat.HasProperty("_DisplacementStrength"))
            {
               var prop = FindProperty("_DisplacementStrength", props);
               if (prop != null)
               {
                  materialEditor.RangeProperty(prop, "Displacement Strength");
               }
            }

            DoStochastic(mat, materialEditor, props, "_STOCHASTIC", "_StochasticContrast", "_StochasticScale");
            EditorGUI.BeginChangeCheck();
            materialEditor.ShaderProperty(FindProperty("_AlphaThreshold", props), "Alpha Clip Threshold");
            if (EditorGUI.EndChangeCheck())
            {
               // because Unity's lightmapper is hard coded to use _Cutoff
               FindProperty("_Cutoff", props).floatValue = FindProperty("_AlphaThreshold", props).floatValue;

               if (GetAlphaState() == AlphaMode.Opaque)
               {
                  if (FindProperty("_AlphaThreshold", props).floatValue > 0)
                  {
                     mat.EnableKeyword("_ALPHACUT");
                     mat.renderQueue = 2450;
                     mat.SetOverrideTag("RenderType", "TransparentCutout");
                  }
                  else
                  {
                     mat.DisableKeyword("_ALPHACUT");
                     mat.renderQueue = -1;
                     mat.SetOverrideTag("RenderType", "");
                  }
               }
            }
            materialEditor.RangeProperty(FindProperty("_MicroShadowStrength", props), "Micro Shadow Strength");
            DoFuzzyShadingUI(materialEditor, props, "", "");



            bool detailRollup = (DrawRollupKeywordToggle(mat, "Detail Texture", "_DETAIL"));
            if (detailRollup)
            {
               EditorGUI.indentLevel++;
               UVMode detailUVMode = DoUVMode(mat, "_DETAILTRIPLANAR", "Detail UV Mode");
               if (detailUVMode == UVMode.UV)
               {
                  var uvSpace = DoUVSource(mat, "_DetailUVSource");

                  if (uvSpace != UVSource.UV0 && uvSpace != UVSource.UV1)
                  {
                     EditorGUI.BeginChangeCheck();
                     TriplanarSpace space = TriplanarSpace.World;
                     if (mat.GetFloat("_DetailTriplanarSpace") > 0.5)
                        space = TriplanarSpace.Local;
                     space = (TriplanarSpace)EditorGUILayout.EnumPopup("Projection Space", space);
                     if (EditorGUI.EndChangeCheck())
                     {
                        mat.SetFloat("_DetailTriplanarSpace", (int)space);
                        EditorUtility.SetDirty(mat);
                     }
                  }
                  
               }
               else
               {
                  DoTriplanarSpace(mat, materialEditor, props, "_DetailTriplanarSpace", "_DetailTriplanarContrast");
               }
               var detailTex = FindProperty("_DetailMap", props);
               if (mat.IsKeywordEnabled("_DETAIL"))
               {
                  if (detailTex.textureValue == null)
                  {
                     detailTex.textureValue = FindDefaultTexture("betterlit_default_detail");
                  }
                  WarnLinear(detailTex.textureValue);
               }
               materialEditor.TexturePropertySingleLine(new GUIContent("Detail"), detailTex);
               materialEditor.TextureScaleOffsetProperty(FindProperty("_DetailMap", props));
               materialEditor.ShaderProperty(FindProperty("_DetailAlbedoStrength", props), new GUIContent("Detail Albedo Strength"));
               if (normalMode != NormalMode.FromHeight)
               {
                  materialEditor.ShaderProperty(FindProperty("_DetailNormalStrength", props), new GUIContent("Detail Normal Strength"));
               }
               materialEditor.ShaderProperty(FindProperty("_DetailSmoothnessStrength", props), new GUIContent("Detail Smoothness Strength"));
               DoStochastic(mat, materialEditor, props, "_DETAILSTOCHASTIC", "_DetailStochasticContrast", "_DetailStochasticScale");
               EditorGUI.indentLevel--;
            }
            
         }
         
      }

      public void OnTessGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (mat != null && mat.HasProperty("_TessellationDisplacement") && mat.HasProperty("_TessellationMaxSubdiv") &&
            materialEditor != null && props != null)
         {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(FindProperty("_TessellationDisplacement", props), "Displacement");
            if (mat.IsKeywordEnabled("_TESSEDGE"))
            {
               materialEditor.ShaderProperty(FindProperty("_TessellationMinEdgeLength", props), "Min Edge Length");
            }
            else
            {
               Vector2 dist = mat.GetVector("_TessellationDistanceRange");

               EditorGUI.BeginChangeCheck();
               dist = EditorGUILayout.Vector2Field("Fade Start/Falloff", dist);

               if (EditorGUI.EndChangeCheck())
               {
                  FindProperty("_TessellationDistanceRange", props).vectorValue = dist;
               }
            }

            
            materialEditor.ShaderProperty(FindProperty("_TessellationMaxSubdiv", props), "Max Subdivisions");
            materialEditor.ShaderProperty(FindProperty("_TessellationMipBias", props), "Mip Bias");
            materialEditor.ShaderProperty(FindProperty("_TessellationOffset", props), "Offset");
            EditorGUI.indentLevel--;
         }
      }


      enum LocalGlobalMode
      {
         Local = 0,
         Global = 1
      }

      public void DoWetness(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_WetnessAmount"))
            return;
         if (DrawRollupKeywordToggle(mat, "Wetness", "_WETNESS"))
         {
            LocalGlobalMode mode = (LocalGlobalMode)mat.GetInt("_WetnessMode");
            EditorGUI.BeginChangeCheck();
            mode = (LocalGlobalMode)EditorGUILayout.EnumPopup("Wetness Mode", mode);

            if (EditorGUI.EndChangeCheck())
            {
               mat.SetInt("_WetnessMode", (int)mode);
            }
            EditorGUI.indentLevel++;
            var old = GUI.enabled;
            GUI.enabled = mode == LocalGlobalMode.Local;
            materialEditor.ShaderProperty(FindProperty("_WetnessAmount", props), "Wetness Amount");
            GUI.enabled = old;

            materialEditor.ShaderProperty(FindProperty("_WetnessMin", props), "Wetness Min");
            materialEditor.ShaderProperty(FindProperty("_WetnessMax", props), "Wetness Max");
            materialEditor.ShaderProperty(FindProperty("_WetnessFalloff", props), "Wetness Falloff");
            materialEditor.ShaderProperty(FindProperty("_WetnessAngleMin", props), "Wetness Angle Minimum");
            var shore = FindProperty("_WetnessShoreline", props);
            EditorGUILayout.BeginHorizontal();
            bool on = false;
            if (shore.floatValue > -9990)
            {
               on = true;
            }
            var newOn = EditorGUILayout.Toggle("Wetness Shore Height", on);
            if (newOn != on)
            {
               if (newOn)
                  shore.floatValue = 0;
               else
                  shore.floatValue = -9999;
               on = newOn;
            }
            var oldEnabled = GUI.enabled;
            GUI.enabled = on;
            if (on)
            {
               float nv = EditorGUILayout.FloatField(shore.floatValue);
               if (nv != shore.floatValue)
               {
                  shore.floatValue = nv;
               }
            }
            else
            {
               EditorGUILayout.FloatField(0);
            }
            GUI.enabled = oldEnabled;
            EditorGUILayout.EndHorizontal();
            materialEditor.ShaderProperty(FindProperty("_Porosity", props), "Porosity");
          
            EditorGUI.indentLevel--;
         }
      }

      public void DoSnow(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_SnowMode"))
            return;
         if (DrawRollupKeywordToggle(mat, "Snow", "_SNOW"))
         {
            LocalGlobalMode mode = (LocalGlobalMode)mat.GetInt("_SnowMode");
            EditorGUI.BeginChangeCheck();
            mode = (LocalGlobalMode)EditorGUILayout.EnumPopup("Snow Mode", mode);

            if (EditorGUI.EndChangeCheck())
            {
               mat.SetInt("_SnowMode", (int)mode);
            }
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            materialEditor.TexturePropertySingleLine(new GUIContent("Snow Albedo"), FindProperty("_SnowAlbedo", props), FindProperty("_SnowTint", props));
            materialEditor.TextureScaleOffsetProperty(FindProperty("_SnowAlbedo", props));
            if (!mat.IsKeywordEnabled("_AUTONORMAL"))
            {
               if (mat.IsKeywordEnabled("_PACKEDFAST"))
               {
                  WarnLinear(FindProperty("_SnowNormal", props).textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Snow Packed"), FindProperty("_SnowNormal", props));
               }
               else
               {
                  WarnNormal(FindProperty("_SnowNormal", props).textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Snow Normal"), FindProperty("_SnowNormal", props));
               }

            }
            if (!mat.IsKeywordEnabled("_PACKEDFAST"))
            {
               WarnLinear(FindProperty("_SnowMaskMap", props).textureValue);
               materialEditor.TexturePropertySingleLine(new GUIContent("Snow Mask"), FindProperty("_SnowMaskMap", props));
            }

            if (mode == LocalGlobalMode.Local)
            {
               materialEditor.ShaderProperty(FindProperty("_SnowAmount", props), "Snow Amount");
            }
            DoStochastic(mat, materialEditor, props, "_SNOWSTOCHASTIC", "_SnowStochasticContrast", "_SnowStochasticScale");
            materialEditor.ShaderProperty(FindProperty("_SnowAngle", props), "Snow Angle Falloff");
            materialEditor.ShaderProperty(FindProperty("_SnowContrast", props), "Snow Contrast");

            Vector3 worldData = mat.GetVector("_SnowWorldFade");
            EditorGUI.BeginChangeCheck();
            worldData.z = (EditorGUILayout.Toggle("World Height Fade", worldData.z > 0 ? true : false)) ? 1 : 0;
            bool old = GUI.enabled;
            GUI.enabled = worldData.z > 0;
            EditorGUI.indentLevel++;
            worldData.x = EditorGUILayout.FloatField("Start Height", worldData.x);
            worldData.y = EditorGUILayout.FloatField("Fade In Range", worldData.y);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
               mat.SetVector("_SnowWorldFade", worldData);
               EditorUtility.SetDirty(mat);
            }
            GUI.enabled = old;
            materialEditor.ShaderProperty(FindProperty("_SnowVertexHeight", props), "Snow Vertex Offset");
            if (EditorGUI.EndChangeCheck())
            {
               if (mat.GetTexture("_SnowMaskMap") != null)
               {
                  mat.EnableKeyword("_SNOWMASKMAP");
               }
               else
               {
                  mat.DisableKeyword("_SNOWMASKMAP");
               }
               if (mat.GetTexture("_SnowNormal") != null)
               {
                  mat.EnableKeyword("_SNOWNORMALMAP");
               }
               else
               {
                  mat.DisableKeyword("_SNOWNORMALMAP");
               }
               
            }

            bool noise = mat.IsKeywordEnabled("_SNOWNOISE");
            EditorGUI.BeginChangeCheck();
            noise = EditorGUILayout.Toggle("Transition Noise", noise);
            if (EditorGUI.EndChangeCheck())
            {
               mat.DisableKeyword("_SNOWNOISE");
               if (noise)
               {
                  mat.EnableKeyword("_SNOWNOISE");
               }
            }
            if (noise)
            {
               EditorGUI.indentLevel++;
               materialEditor.FloatProperty(FindProperty("_SnowNoiseFreq", props), "Frequency");
               materialEditor.FloatProperty(FindProperty("_SnowNoiseAmp", props), "Amplitude");
               materialEditor.FloatProperty(FindProperty("_SnowNoiseOffset", props), "Offset");
               EditorGUI.indentLevel--;
            }

            if (mat.IsKeywordEnabled("_TRAX_ON"))
            {
               materialEditor.TexturePropertySingleLine(new GUIContent("Trax Snow Albedo"), FindProperty("_SnowTraxAlbedo", props), FindProperty("_SnowTraxTint", props));
               materialEditor.TextureScaleOffsetProperty(FindProperty("_SnowTraxAlbedo", props));
               if (!mat.IsKeywordEnabled("_AUTONORMAL"))
               {
                  if (mat.IsKeywordEnabled("_PACKEDFAST"))
                  {
                     WarnLinear(FindProperty("_SnowTraxNormal", props).textureValue);
                     materialEditor.TexturePropertySingleLine(new GUIContent("Trax Snow Packed"), FindProperty("_SnowTraxNormal", props));
                  }
                  else
                  {
                     WarnNormal(FindProperty("_SnowTraxNormal", props).textureValue);
                     materialEditor.TexturePropertySingleLine(new GUIContent("Trax Snow Normal"), FindProperty("_SnowTraxNormal", props));
                  }

               }
               if (!mat.IsKeywordEnabled("_PACKEDFAST"))
               {
                  WarnLinear(FindProperty("_SnowTraxMaskMap", props).textureValue);
                  materialEditor.TexturePropertySingleLine(new GUIContent("Trax Snow Mask"), FindProperty("_SnowTraxMaskMap", props));
               }
            }
            EditorGUI.indentLevel--;
         }
      }


      enum DebugMode
      {
         Off,
         SampleCount
      }

      public void DoDebugGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!mat.HasProperty("_DebugSampleCountThreshold"))
            return;
         DebugMode mode = DebugMode.Off;
         if (mat.IsKeywordEnabled("_DEBUG_SAMPLECOUNT"))
         {
            mode = DebugMode.SampleCount;
         }
         var nm = (DebugMode)EditorGUILayout.EnumPopup("Debug Mode", mode);
         if (nm != mode)
         {
            mat.DisableKeyword("_DEBUG_SAMPLECOUNT");
            if (nm == DebugMode.SampleCount)
            {
               mat.EnableKeyword("_DEBUG_SAMPLECOUNT");
            }
         }
         if (nm == DebugMode.SampleCount)
         {
            EditorGUILayout.HelpBox("The shader will draw red when texture samples are greater than the Debug Sample Threshold, and blue when below it. This can let you see exactly how many samples are needed for the shader based on it's current configuration", MessageType.Info);
            materialEditor.ShaderProperty(FindProperty("_DebugSampleCountThreshold", props), "Debug Sample Threshold");
         }

      }

      public void OnEffectsGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      { 
         DoWetness(materialEditor, props);
         DoPuddles(materialEditor, props);
         DoSnow(materialEditor, props);
         DoDebugGUI(materialEditor, props);
      }
   }
}