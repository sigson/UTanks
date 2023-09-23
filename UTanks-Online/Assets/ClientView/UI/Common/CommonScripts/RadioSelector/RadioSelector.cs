using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.Special
{
    public class RadioSelector : ValueStorage
    {
        [SerializeField] private RadioSelectorHandler selectorHandler;
        [SerializeField] private TMP_Text selectorVisualValue;
        [SerializeField] private string RadioSelectorValue;

        public void SelectValue()
        {
            selectorHandler.SelectedValue = this;
        }

        protected override object ValueGetDecoration(object presentedValue)
        {
            return RadioSelectorValue;
        }

        protected override void ValueSetDecoration(object newValue)
        {
            RadioSelectorValue = (string)newValue;
            selectorVisualValue.GetAdapter().text = (string)newValue;
        }
    }
}