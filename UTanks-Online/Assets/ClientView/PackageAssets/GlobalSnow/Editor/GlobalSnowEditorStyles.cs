using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GlobalSnowEffect {

    public static class GlobalSnowEditorStyles {

		public static void SetFoldoutColor(this GUIStyle style) {
			Color foldoutColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
			style.normal.textColor = foldoutColor;
			style.onNormal.textColor = foldoutColor;
			style.hover.textColor = foldoutColor;
			style.onHover.textColor = foldoutColor;
			style.focused.textColor = foldoutColor;
			style.onFocused.textColor = foldoutColor;
			style.active.textColor = foldoutColor;
			style.onActive.textColor = foldoutColor;
			style.fontStyle = FontStyle.Bold;
		}
	}
}


