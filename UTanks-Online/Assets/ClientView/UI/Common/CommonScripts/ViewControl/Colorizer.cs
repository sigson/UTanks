using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    public class Colorizer : UIBehaviour
    {
        public List<UIBehaviour> colorizationObjects = new List<UIBehaviour>();
        public Material material;
        public Color color = Color.white;
        public Color ShadeColor = Color.white;
        [Range(0f, 1f)]
        public float ColorShadingValue;

        public void UpdateColor(Color? _color = null)
        {
            if (_color != null)
                color = (Color)_color;
            foreach (var graphic in colorizationObjects)
            {
                if (graphic == null)
                    continue;
                if (graphic is Colorizer)
                {
                    ((Colorizer)graphic).color = Color.Lerp(this.color, ShadeColor, ColorShadingValue);
                    ((Colorizer)graphic).UpdateColor();
                }
                else
                {
                    var _graphic = (Graphic)graphic;
                    _graphic.color = Color.Lerp(this.color, ShadeColor, ColorShadingValue);
                    if (this.material != null)
                        _graphic.material = this.material;
                }
            }
        }

        public void FindChildGraphic()
        {
            colorizationObjects.Clear();
            foreach (var graphic in GetComponentsInChildren<Graphic>(false))
            {
                colorizationObjects.Add(graphic);
                UpdateColor();
            }
            foreach (var graphic in GetComponentsInChildren<Colorizer>(false))
            {
                if (graphic == this)
                    continue;
                colorizationObjects.Add(graphic);
                UpdateColor();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Colorizer))]
    public class ColorizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool generate = false;
            if (GUILayout.Button("Generate graphic child", GUILayout.Height(30)))
            {
                generate = true;
            }

            var script = (Colorizer)target;
            if (script.enabled)
            {
                if (generate)
                {
                    script.FindChildGraphic();
                }
                script.UpdateColor();
            }

            //EditorGUI.BeginChangeCheck();

            //if (EditorGUI.EndChangeCheck())
            //{

            //    //Repaint();
            //}
        }
    }
#endif
}