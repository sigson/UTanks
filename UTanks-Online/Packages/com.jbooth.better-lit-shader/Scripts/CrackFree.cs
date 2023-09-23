using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.BetterLit
{
   [ExecuteInEditMode]
   public class CrackFree : MonoBehaviour
   {
      public float weldThreshold = 0.001f;

      private void OnEnable()
      {
         StampDampening();
      }

      void StampDampening()
      {
         MeshFilter mf = GetComponent<MeshFilter>();
         Mesh mesh = mf.sharedMesh;
         if (mesh.GetVertexAttributeDimension(UnityEngine.Rendering.VertexAttribute.TexCoord0) == 3)
         {
            return;
         }
         List<Vector3> verts = new List<Vector3>(mesh.vertexCount);
         List<Vector3> normals = new List<Vector3>(mesh.vertexCount);
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
      }
   }
}
