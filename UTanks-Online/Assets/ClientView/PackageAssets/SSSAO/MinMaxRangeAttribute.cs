using UnityEngine;

public class MinMaxRangeAttribute : PropertyAttribute {
    public float min;
    public float max;
    
    public MinMaxRangeAttribute (float min, float max) {
        this.min = min;
        this.max = max;
    }
}

