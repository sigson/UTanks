using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JBooth.InlineTexturePacker
{

   class InlineTexturePackerWindow : EditorWindow
   {
      [MenuItem("Window/Better Lit Shader/Pack Color and Height Texture")]
      static void PackColorHeight()
      {
         InlineTexturePackerWindow.DoWindow(Mode.ColorA, "", "", "", "Height");
      }

      [MenuItem("Window/Better Lit Shader/Pack Normal Smoothness AO")]
      static void PackNormal()
      {
         InlineTexturePackerWindow.DoWindow(Mode.NormalRB, "Smoothness", "", "AO", "");
      }

      [MenuItem("Window/Better Lit Shader/Pack Mask Map")]
      static void PackMask()
      {
         InlineTexturePackerWindow.DoWindow(Mode.FourChannel, "Metalic", "Occlusion", "Detail", "Smoothness");
      }

      [MenuItem("Window/Better Lit Shader/Pack Detail Texture")]
      static void PackDetail()
      {
         InlineTexturePackerWindow.DoWindow(Mode.Detail, "Albedo", "", "Smoothness", "");
      }

      static Texture2D lastResult;
      enum Channel
      {
         Red = 0, Green, Blue, Alpha
      }

      public enum Mode
      {
         NormalRB,      // normal map plus channels in R/B
         FourChannel,   // four single channels
         ColorA,        // color plus data in alpha
         Detail         // HDRP style detail texture 
      }

      class TextureData
      {
         public Texture2D tex;
         public Channel channel;
         public bool invert;
      }


      string R, G, B, A;
      TextureData texR = new TextureData();
      TextureData texG = new TextureData();
      TextureData texB = new TextureData();
      TextureData texA = new TextureData();
      TextureData texColor = new TextureData();
      TextureData texNormal = new TextureData();

      Mode mode;

      public static void DoWindow(Mode mode, string R, string G, string B, string A)
      {
         InlineTexturePackerWindow window = (InlineTexturePackerWindow)EditorWindow.GetWindow<InlineTexturePackerWindow>(true, "TexturePacker", true);
         Vector2 size = new Vector2(430, 190);
         if (mode == Mode.NormalRB || mode == Mode.Detail)
         {
            size = new Vector2(330, 190);
         }
         else if (mode == Mode.ColorA)
         {
            size = new Vector2(230, 190);
         }
         window.minSize = size;
         window.maxSize = size;
         window.mode = mode;
         window.R = R;
         window.G = G;
         window.B = B;
         window.A = A;
         lastResult = null;

      }

      void TextureGUI(ref TextureData d, string label, bool isChannel)
      {
         GUILayout.BeginVertical();
         var style = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
         style.alignment = TextAnchor.UpperCenter;
         style.fixedWidth = 100;
         GUILayout.Label(label, style);
         d.tex = (Texture2D)EditorGUILayout.ObjectField(d.tex, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
         if (isChannel)
         {
            d.channel = (Channel)EditorGUILayout.EnumPopup(d.channel, GUILayout.Width(100));
            EditorGUILayout.BeginHorizontal();
            d.invert = EditorGUILayout.Toggle(d.invert, GUILayout.Width(20));
            EditorGUILayout.LabelField("Invert", GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
         }
         GUILayout.EndVertical();
      }

      static string lastDir;

      TextureImporterCompression Uncompress(Texture2D tex)
      {
         var ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
         var ret = ti.textureCompression;
         ti.textureCompression = TextureImporterCompression.Uncompressed;
         ti.SaveAndReimport();
         return ret;
      }

      void Compress(Texture2D tex, TextureImporterCompression cmp)
      {
         var ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
         ti.textureCompression = cmp;
         ti.SaveAndReimport();
      }

      Vector2 FindLargestTextureSize()
      {
         Vector2 largest = Vector2.zero;
         if (texR.tex != null)
         {
            largest.x = texR.tex.width;
            largest.y = texR.tex.height;
         }
         if (texG.tex != null)
         {
            if (texG.tex.width > largest.x)
               largest.x = texG.tex.width;
            if (texG.tex.height > largest.y)
               largest.y = texG.tex.height;
         }
         if (texB.tex != null)
         {
            if (texB.tex.width > largest.x)
               largest.x = texB.tex.width;
            if (texB.tex.height > largest.y)
               largest.y = texB.tex.height;
         }
         if (texA.tex != null)
         {
            if (texA.tex.width > largest.x)
               largest.x = texA.tex.width;
            if (texA.tex.height > largest.y)
               largest.y = texA.tex.height;
         }
         if (texNormal.tex != null)
         {
            if (texNormal.tex.width > largest.x)
               largest.x = texNormal.tex.width;
            if (texNormal.tex.height > largest.y)
               largest.y = texNormal.tex.height;
         }
         if (texColor.tex != null)
         {
            if (texColor.tex.width > largest.x)
               largest.x = texColor.tex.width;
            if (texColor.tex.height > largest.y)
               largest.y = texColor.tex.height;
         }
         return largest;
      }

      void ReadBackTexture(RenderTexture rt, Texture2D tex)
      {
         var old = RenderTexture.active;
         RenderTexture.active = rt;
         tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
         tex.Apply();
         RenderTexture.active = old;
      }

      void ExtractChannel(RenderTexture rt, Texture2D tex, Channel src, Channel dst, bool invert, bool grey = false)
      {
         Texture2D temp = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true, mode != Mode.ColorA);
         ReadBackTexture(rt, temp);
         Color[] srcC = temp.GetPixels();
         Color[] destC = tex.GetPixels();
         if (grey)
         {
            for (int i = 0; i < srcC.Length; ++i)
            {
               float g = srcC[i].grayscale;
               srcC[i] = new Color(g, g, g, g);
            }
         }
         
         for (int i = 0; i < srcC.Length; ++i)
         {
            Color sc = srcC[i];
            Color dc = destC[i];

            dc[(int)dst] = sc[(int)src];
            destC[i] = dc;
         }

         if (invert)
         {
            for (int i = 0; i < destC.Length; ++i)
            {
               Color dc = destC[i];
               dc[(int)dst] = 1.0f - dc[(int)src];
               destC[i] = dc;
            }
         }
         tex.SetPixels(destC);
         DestroyImmediate(temp);
      }

      void Save()
      {
         Vector2 largest = FindLargestTextureSize();
         if (largest.x < 1)
         {
            Debug.LogError("You need some textures in there");
            return;
         }

         string path = EditorUtility.SaveFilePanel("Save Resulting Texture", lastDir, "", "tga");
         if (string.IsNullOrEmpty(path))
            return;

         Texture2D tex = new Texture2D((int)largest.x, (int)largest.y, TextureFormat.ARGB32, true, mode != Mode.ColorA);

         RenderTexture rt = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGB32, mode == Mode.ColorA ?  RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
         Graphics.Blit(Texture2D.blackTexture, rt);
         
         if (mode == Mode.NormalRB)
         {
            if (texNormal.tex != null)
            {
               var cmp = Uncompress(texNormal.tex);
               Graphics.Blit(texNormal.tex, rt);
               ReadBackTexture(rt, tex);
               Compress(texNormal.tex, cmp);
            }
            if (texR.tex != null)
            {
               var cmp = Uncompress(texR.tex);
               Graphics.Blit(texR.tex, rt);
               ExtractChannel(rt, tex, texR.channel, Channel.Red, texR.invert);
               Compress(texR.tex, cmp);
            }
            if (texB.tex != null)
            {
               var cmp = Uncompress(texB.tex);
               Graphics.Blit(texB.tex, rt);
               ExtractChannel(rt, tex, texB.channel, Channel.Blue, texB.invert);
               Compress(texR.tex, cmp);
            }
         }
         else if (mode == Mode.FourChannel)
         {
            if (texR.tex != null)
            {
               var cmp = Uncompress(texR.tex);
               Graphics.Blit(texR.tex, rt);
               ExtractChannel(rt, tex, texR.channel, Channel.Red, texR.invert);
               Compress(texR.tex, cmp);
            }
            if (texG.tex != null)
            {
               var cmp = Uncompress(texG.tex);
               Graphics.Blit(texG.tex, rt);
               ExtractChannel(rt, tex, texG.channel, Channel.Green, texG.invert);
               Compress(texG.tex, cmp);
            }
            if (texB.tex != null)
            {
               var cmp = Uncompress(texB.tex);
               Graphics.Blit(texB.tex, rt);
               ExtractChannel(rt, tex, texB.channel, Channel.Blue, texB.invert);
               Compress(texB.tex, cmp);
            }
            if (texA.tex != null)
            {
               var cmp = Uncompress(texA.tex);
               Graphics.Blit(texA.tex, rt);
               ExtractChannel(rt, tex, texA.channel, Channel.Alpha, texA.invert);
               Compress(texA.tex, cmp);
            }
         }
         else if (mode == Mode.ColorA)
         {
            if (texColor.tex != null)
            {
               var cmp = Uncompress(texColor.tex);
               Graphics.Blit(texColor.tex, rt);
               ReadBackTexture(rt, tex);
               Compress(texColor.tex, cmp);
            }
            if (texA.tex != null)
            {
               var cmp = Uncompress(texA.tex);
               Graphics.Blit(texA.tex, rt);
               ExtractChannel(rt, tex, texA.channel, Channel.Alpha, texA.invert);
               Compress(texA.tex, cmp);
            }
         }
         else if (mode == Mode.Detail)
         {
            if (texNormal.tex != null)
            {
               var cmp = Uncompress(texNormal.tex);
               Graphics.Blit(texNormal.tex, rt);
               ReadBackTexture(rt, tex);
               Compress(texNormal.tex, cmp);
            }
            if (texColor.tex != null)
            {
               var cmp = Uncompress(texColor.tex);
               Graphics.Blit(texColor.tex, rt);
               ExtractChannel(rt, tex, texColor.channel, Channel.Red, false, true);
               Compress(texColor.tex, cmp);
            }
            if (texB.tex != null)
            {
               var cmp = Uncompress(texB.tex);
               Graphics.Blit(texB.tex, rt);
               ExtractChannel(rt, tex, texB.channel, Channel.Blue, texB.invert);
               Compress(texB.tex, cmp);
            }
         }
         tex.Apply(true, false);
         var tga = tex.EncodeToTGA();
         System.IO.File.WriteAllBytes(path, tga);
         DestroyImmediate(rt);
         DestroyImmediate(tex);
         AssetDatabase.Refresh();
         path = path.Substring(path.IndexOf("Assets"));
         if (mode != Mode.ColorA)
         {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            TextureImporter ti = (TextureImporter)ai;
            ti.sRGBTexture = mode == Mode.ColorA;
            ti.SaveAndReimport();
         }

         lastResult = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
         EditorApplication.delayCall += () =>
         {
            Close();
         };

      }

      private void OnGUI()
      {
         using (new GUILayout.VerticalScope(EditorStyles.helpBox))
         {
            using (new GUILayout.HorizontalScope())
            {
               if (mode == Mode.NormalRB)
               {
                  TextureGUI(ref texNormal, "Normal", false);
                  TextureGUI(ref texR, R, true);
                  TextureGUI(ref texB, B, true);
               }
               else if (mode == Mode.FourChannel)
               {
                  TextureGUI(ref texR, R, true);
                  TextureGUI(ref texG, G, true);
                  TextureGUI(ref texB, B, true);
                  TextureGUI(ref texA, A, true);
               }
               else if (mode == Mode.ColorA)
               {
                  TextureGUI(ref texColor, "Color", false);
                  TextureGUI(ref texA, A, true);
               }
               else if (mode == Mode.Detail)
               {
                  TextureGUI(ref texColor, "Color", false);
                  TextureGUI(ref texNormal, "Normal", false);
                  TextureGUI(ref texB, B, true);
               }
            }

            if (GUILayout.Button("Save"))
            {
               Save();
            }
         }

      }

   }

}
