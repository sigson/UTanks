using UnityEditor;
using UnityEngine;

// Tell the RangeDrawer that it is a drawer for properties with the RangeAttribute.
[CustomPropertyDrawer (typeof (MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer {
    
    // Draw the property inside the given rect
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        
        // First get the attribute since it contains the range for the slider
        MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
        
        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        Vector2 newRange = property.vector2Value;
        EditorGUI.MinMaxSlider (label, position,ref newRange.x, ref newRange.y, range.min, range.max);
        property.vector2Value = newRange;
    }
}

