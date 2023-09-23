//////////////////////////////////////////////////////
// Better Lit Shader
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JBooth.BetterLit
{
   public class CrackFreeProcessor : EditorWindow
   {
      [MenuItem("Window/Better Lit Shader/Crack Free Tessellation Mesh Processor")]
      public static void ShowWindow()
      {
         var window = GetWindow<CrackFreeProcessor>();
         window.Show();
      }
      public GameObject obj;
      public float weldThreshold = 0.001f;

      public void OnGUI()
      {
         obj = (GameObject)EditorGUILayout.ObjectField("Game Object", obj, typeof(GameObject), false);
         weldThreshold = EditorGUILayout.Slider("Weld Threashold", weldThreshold, 0.0001f, 0.02f);
         if (GUILayout.Button("Process"))
         {
            string path = "";
            var assets = new List<Mesh>();
            if (obj != null)
            {
               path = AssetDatabase.GetAssetPath(obj);
               var mfs = obj.GetComponentsInChildren<MeshFilter>();
               for (int i = 0; i < mfs.Length; ++i)
               {
                  Mesh m = mfs[i].sharedMesh;
                  if (m != null)
                  {
                     Mesh result = Process(m);
                     if (result != null)
                        assets.Add(result);
                  }
               }
            }

            if (assets.Count > 0)
            {

               path = path.Substring(0, path.IndexOf("."));
               path += "_crackfree.asset";

               AssetDatabase.CreateAsset(assets[0], path);
               for (int i = 1; i < assets.Count; ++i)
               {
                  AssetDatabase.AddObjectToAsset(assets[i], path);
               }
               AssetDatabase.SaveAssets();
            }
         }
      }

      public Mesh Process(Mesh mesh)
      {
         mesh = Instantiate(mesh);
         List<Vector3> verts = new List<Vector3>(mesh.vertexCount);
         List<Vector2> uvs = new List<Vector2>(mesh.vertexCount);
         Vector3[] dispUVs = new Vector3[mesh.vertexCount];
         int[] triangles = mesh.triangles;
         
         mesh.GetVertices(verts);
         mesh.GetUVs(0, uvs);
         List<int> search = new List<int>();
         for (int x = 0; x < verts.Count; ++x)
         {
            Vector3 orig = verts[x];
            Vector2 uv = uvs[x];
            float dampen = 0;
            int count = 1;
            // index, count
            for (int y = 0; y < verts.Count; ++y)
            {
               Vector3 comp = verts[y];
               if (x != y && (Vector3.Distance(orig, comp) < weldThreshold))
               {
                  count++;
                  if (Vector2.Distance(uv, uvs[y]) > weldThreshold)
                  {
                     dampen = 1;
                  }
               }
            }
            if (count > 4 && dampen > 0)
            {
               search.Add(x);
            }
            dispUVs[x] = new Vector3(uv.x, uv.y, dampen);
         }
         foreach (var index in search)
         {
            for (int t = 0; t < triangles.Length; t = t + 3)
            {
               int idx0 = triangles[t];
               int idx1 = triangles[t + 1];
               int idx2 = triangles[t + 2];

               if (idx0 == index || idx1 == index || idx2 == index)
               {
                  dispUVs[idx0].z = 1;
                  dispUVs[idx1].z = 1;
                  dispUVs[idx2].z = 1;
               }
            }
         }
         mesh.SetUVs(0, dispUVs);
         return mesh;
      }
   }
}