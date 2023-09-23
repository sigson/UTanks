using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.Controls
{
    public class InputFieldValueTreshold : MonoBehaviour
    {
        public bool isActive = false;
        public float Min;
        public float Max;

        private void Awake()
        {
            if (!isActive)
                return;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null)
            {
                inputField.onValueChanged.AddListener<string>((input) =>
                {
                    if (Lambda.TryExecute(() => input.FastFloat()))
                    {
                        if (input.FastFloat() > Max)
                        {
                            inputField.text = Mathf.FloorToInt(Max).ToString();
                        }
                        if (input.FastFloat() < Min)
                        {
                            inputField.text = Mathf.FloorToInt(Min).ToString();
                        }
                        inputField.text = Mathf.FloorToInt(inputField.text.FastFloat()).ToString();
                    }
                });
            }
            TMP_InputField TMPinputField = this.GetComponent<TMP_InputField>();
            if (TMPinputField != null)
            {
                TMPinputField.onValueChanged.AddListener<string>((input) =>
                {
                    if (Lambda.TryExecute(() => input.FastFloat()))
                    {
                        if (input.FastFloat() > Max)
                        {
                            TMPinputField.text = Mathf.FloorToInt(Max).ToString();
                        }
                        if (input.FastFloat() < Min)
                        {
                            TMPinputField.text = Mathf.FloorToInt(Min).ToString();
                        }
                        TMPinputField.text = Mathf.FloorToInt(TMPinputField.text.FastFloat()).ToString();
                    }
                });
            }
        }
    }
}