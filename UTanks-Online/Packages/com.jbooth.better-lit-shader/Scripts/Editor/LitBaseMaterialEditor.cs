//////////////////////////////////////////////////////
// Better Lit Shader
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

// This is the material editor for the resulting shader. Basically it
// calls into the editor stub for all the various editors rather than
// having tj

using UnityEngine;
using UnityEditor;

namespace JBooth.BetterLit
{
   public class LitBaseMaterialEditor : ShaderGUI
   {
      static Texture2D gradient;
      static Texture2D logo;

      LitBaseStub stub = null;
      
      public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         
         var mat = materialEditor.target as Material;
         
         if (stub == null)
         {
            stub = new LitBaseStub(this, mat);
         }
         if (gradient == null)
         {
            gradient = Resources.Load<Texture2D>("betterlit_gradient");
            logo = Resources.Load<Texture2D>("betterlit_logo");
         }


         var rect = EditorGUILayout.GetControlRect(GUILayout.Height(32));
         EditorGUI.DrawPreviewTexture(rect, gradient);
         GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit, true);
         //EditorGUI.DrawTextureTransparent(rect, logo);

         if (LitBaseStub.DrawRollup("Settings"))
         {
            stub.OnLitShaderSettings(materialEditor, props);
            stub.DoAlphaOptions();       // may change the shader
            stub.DoCullMode();
            stub.DoNormalMode(materialEditor, props);
            stub.DoFlatShadingMode(materialEditor, props);
         }
         stub.OnLitGUI(materialEditor, props);
         stub.DoTintMask(materialEditor, props);
         stub.DoTextureLayerWeights(materialEditor, props);
         stub.DoTextureLayer(materialEditor, props, stub.GetPacking(), 0);
         stub.DoTextureLayer(materialEditor, props, stub.GetPacking(), 1);
         stub.DoTextureLayer(materialEditor, props, stub.GetPacking(), 2);

         stub.DoWetness(materialEditor, props);
         stub.DoPuddles(materialEditor, props);
         stub.DoSnow(materialEditor, props);
         stub.DoWind(materialEditor, props);
         stub.DoTrax(materialEditor, props);
         stub.DoDissolve(materialEditor, props);

         stub.DoTessellationOption(materialEditor, props); // can switch shader, check changeShader at the end
         stub.DoBakery(materialEditor, props);

         stub.DoDebugGUI(materialEditor, props);
         if (UnityEngine.Rendering.SupportedRenderingFeatures.active.editableMaterialRenderQueue)
            materialEditor.RenderQueueField();
         materialEditor.EnableInstancingField();
         materialEditor.DoubleSidedGIField();

         if (stub.changeShader != null)
         {
            // sometimes it wipes one when we change shaders
            var keywords = mat.shaderKeywords;
            mat.shader = stub.changeShader;
            stub.changeShader = null;
            mat.shaderKeywords = keywords;
         }
      }
   }
}
