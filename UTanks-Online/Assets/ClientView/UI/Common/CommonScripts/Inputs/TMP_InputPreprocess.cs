using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SecuredSpace.UI.Controls
{
    public class TMP_InputPreprocess : MonoBehaviour
    {
        //public 
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TMP_InputPreprocess))]
    public class TMP_InputPreprocessEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            if (EditorGUI.EndChangeCheck())
            {

                //Repaint();
            }
        }
    }
#endif
}