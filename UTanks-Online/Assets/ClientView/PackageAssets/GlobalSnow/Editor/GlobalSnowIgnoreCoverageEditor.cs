using UnityEngine;
using UnityEditor;


namespace GlobalSnowEffect {
    [CustomEditor(typeof(GlobalSnowIgnoreCoverage))]
    public class GlobalSnowIgnoreCoverageEditor : Editor {

        SerializedProperty receiveSnow, blockSnow, exclusionCutOff;

        private void OnEnable() {
            receiveSnow = serializedObject.FindProperty("_receiveSnow");
            exclusionCutOff = serializedObject.FindProperty("_exclusionCutOff");
            blockSnow = serializedObject.FindProperty("_blockSnow");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(receiveSnow);
            if (receiveSnow.boolValue) {
                GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(exclusionCutOff);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(blockSnow);

            if (serializedObject.ApplyModifiedProperties()) {
                GlobalSnow snow = GlobalSnow.instance;
                if (snow != null) {
                    snow.RefreshExcludedObjects();
                }
            }

        }
    }

}