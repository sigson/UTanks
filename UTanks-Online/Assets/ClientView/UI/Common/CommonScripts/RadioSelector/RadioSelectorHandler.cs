using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.Special
{
    public class RadioSelectorHandler : MonoBehaviour
    {
        [SerializeField]private ValueStorage SelectedValueData = null;
        [SerializeField]public SerializableDictionary<string, RadioSelector> radioVariants = new SerializableDictionary<string, RadioSelector>();
        public UnityAction<RadioSelectorHandler> OnChangeSelected;
        public ValueStorage SelectedValue
        {
            get
            {
                return SelectedValueData;
            }
            set
            {
                SelectedValueData = value;
                OnChangeSelected.Invoke(this);
            }
        }
    }
}