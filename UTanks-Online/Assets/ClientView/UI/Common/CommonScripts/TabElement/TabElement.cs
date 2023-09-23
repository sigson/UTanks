using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;

namespace SecuredSpace.UI.Special
{
    public class TabElement : MonoBehaviour
    {
        public SerializableDictionary<Button, GameObject> TabDB = new SerializableDictionary<Button, GameObject>();

        public void SelectTab(Button button)
        {
            TabDB.Values.ForEach(x => x.SetActive(false));
            TabDB[button].SetActive(true);
        }
    }
}