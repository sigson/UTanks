using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    public class SliderValueIndicator : MonoBehaviour
    {
        public string valuePrefix;
        public string valuePostfix;

        public List<Text> labelsToUpdate = new List<Text>();
        public List<InputField> inputsToUpdate = new List<InputField>();
        public List<TMP_InputField> TMP_inputsToUpdate = new List<TMP_InputField>();
        public List<TMP_Text> TMP_labelsToUpdate = new List<TMP_Text>();
        public bool isNumericMode;

        public void UpdateValue(int value)
        {
            UpdateValue((float)value);
        }

        public void UpdateValue(float value)
        {
            foreach(var label in labelsToUpdate)
            {
                label.text = valuePrefix + value.ToString() + valuePostfix;
            }
            foreach (var label in TMP_labelsToUpdate)
            {
                label.text = valuePrefix + value.ToString() + valuePostfix;
            }
            foreach (var label in TMP_inputsToUpdate)
            {
                label.text = valuePrefix + value.ToString() + valuePostfix;
            }
            foreach (var label in inputsToUpdate)
            {
                label.text = valuePrefix + value.ToString() + valuePostfix;
            }
        }
    }
}