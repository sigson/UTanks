//////////////////////////////////////////////////////
// Better Lit Shader
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

#if __BETTERSHADERS__

using JBooth.BetterShaders;

namespace JBooth.BetterLit
{
   public class StackableTextureLayerWeightEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }

         stub.DoTextureLayerWeights(materialEditor, props);
      }
   }

   public class StackableDissolveEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }

         stub.DoDissolve(materialEditor, props);
      }
   }

   public class StackableTraxEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }

         stub.DoTrax(materialEditor, props);
      }
   }

   public class StackableTintMask : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }

         stub.DoTintMask(materialEditor, props);
      }
   }

   public class StackableWindEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }

         stub.DoWind(materialEditor, props);
      }
   }

   public class StackableTextureLayerMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         var packing = stub.GetPacking();
         stub.DoTextureLayer(materialEditor, props, packing, Mathf.Max(0, stackIndex));
      }
   }

   public class StackableDebugViewMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.DoDebugGUI(materialEditor, props);
      }
   }

   public class StackableDoubleSidedMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.DoCullMode();
      }
   }

   public class StackableSnowMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.DoSnow(materialEditor, props);
      }
   }

   public class StackablePuddlesMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.DoWetness(materialEditor, props);
         stub.DoPuddles(materialEditor, props);
      }
   }

   public class StackableNormalFinalizerMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.DoNormalMode(materialEditor, props);
      }
   }

   public class LitBaseSubMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.OnLitShaderSettings(materialEditor, props);
         stub.DoFlatShadingMode(materialEditor, props);
         stub.OnLitGUI(materialEditor, props);
         
      }
   }

   public class LitTessellationMaterialEditor : SubShaderMaterialEditor
   {
      LitBaseStub stub = null;
      public override void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {
         SubShaderMaterialEditor.DrawHeader("Tessellation");
         if (stub == null)
         {
            stub = new LitBaseStub(shaderGUI, mat);
         }
         stub.OnTessGUI(materialEditor, props);

      }
   }
}
#endif

