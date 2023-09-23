using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    public class ValueIndicator : MonoBehaviour
    {
        public InputField InputFieldResult;
        public Text TextFieldResult;
        public string prefix = "";
        public string suffix = "";

        public List<UIBehaviour> uiElements = new List<UIBehaviour>();
        void Start()
        {
            uiElements = this.GetComponents<UIBehaviour>().ToList();
            foreach(var uiElement in uiElements)
            {
                if(uiElement is InputField)
                {
                    var input = uiElement as InputField;
                    input.onValueChanged.AddListener((string str) =>
                    {
                        if (InputFieldResult != null)
                            InputFieldResult.text = prefix + input.text + suffix;
                        if (TextFieldResult != null)
                            TextFieldResult.text = prefix + input.text + suffix;
                    });
                }
                if (uiElement is Slider)
                {
                    var input = uiElement as Slider;
                    input.onValueChanged.AddListener((float str) =>
                    {
                        if (InputFieldResult != null)
                            InputFieldResult.text = prefix + input.value.ToString() + suffix;
                        if (TextFieldResult != null)
                            TextFieldResult.text = prefix + input.value.ToString() + suffix;
                    });
                }
            }
        }
    }
}