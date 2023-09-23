using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    public class NumericUpDownOperator : MonoBehaviour
    {
        public int updateValue;
        public InputField UpDownText;
        public bool CheckSmallThenZero;
        public void UpdateValue() => UpDownText.text = (CheckSmallThenZero && int.Parse(UpDownText.text == "" ? "0" : UpDownText.text) <= 0 && updateValue < 0) ? (UpDownText.text == "" ? "0" : UpDownText.text) : (int.Parse(UpDownText.text == "" ? "0" : UpDownText.text) + updateValue).ToString();
    }
}