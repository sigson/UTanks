using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace JBooth.BetterLit
{
   [CustomEditor(typeof(BetterLitVariantConfig))]
   public class BetterLitVariantConfigEditor : Editor
   {
      public override void OnInspectorGUI()
      {
         EditorGUILayout.HelpBox("Enabling any of these will completely strip these variants from the build, reducing build size and time", MessageType.Info);
         DrawDefaultInspector();
         
      }
   }
}
