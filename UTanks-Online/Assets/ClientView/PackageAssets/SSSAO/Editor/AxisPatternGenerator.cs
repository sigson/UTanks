using UnityEngine;
using UnityEditor;
using System;

public class AxisPatternGenerator
{
     [MenuItem("Simple SSAO/Generate Axis Pattern Texture...")]
     static void GenerateAxisPattern()
     {
         Texture2D axisPattern = new Texture2D(3,3);

         Color[] colors = axisPattern.GetPixels();

         // 90 degrees / 9 samples = 10 degrees increment. 
         // interleave increments per row and column so that they are evenly spread out.
            
         float[] angles = new float[9]{
            0,  30, 60,
            40, 70, 10,
            80, 50, 20
         };

        float[] lenghts = new float[9]{
            0.7f,  0.4f,   0.2f,
            0.3f,  0.5f,   0.9f,
            0.0f,  0.6f,   0.1f
        };

         for (int i = 0; i < 9; ++i){
            Vector2 axis = Quaternion.Euler(0, 0, angles[i]) * Vector2.right;
            colors[i] = new Color(axis.x,axis.y,lenghts[i],1);
        }
         
         axisPattern.SetPixels(colors);
         axisPattern.Apply();

         AssetDatabase.CreateAsset(axisPattern, "Assets/GameData/PackageAssets/SSSAO/Resources/AxisPattern.asset");
     }
}


