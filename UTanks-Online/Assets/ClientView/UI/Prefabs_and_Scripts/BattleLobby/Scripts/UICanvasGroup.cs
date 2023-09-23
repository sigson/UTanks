using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.UI.GameUI
{
    public class UICanvasGroup : MonoBehaviour
    {
        public SerializableDictionary<string, GameObject> CanvasGroup = new SerializableDictionary<string, GameObject>();
    }
}