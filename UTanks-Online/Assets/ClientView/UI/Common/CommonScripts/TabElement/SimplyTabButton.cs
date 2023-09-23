using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;

namespace SecuredSpace.UI.Special
{
    public class SimplyTabButton : MonoBehaviour
    {
        [SerializeField] private GameObject[] Deactivate;
        [SerializeField] private GameObject[] Activate;
        public bool AutoAppendToLocalButton = false;

        private void Start()
        {
            if(AutoAppendToLocalButton)
            {
                this.GetComponents<Button>().ForEach(x =>
                {
                    x.onClick.RemoveAllListeners();
                    x.onClick.AddListener(TabSelected);
                });
            }
        }

        private void TabSelected()
        {
            if (Deactivate != null)
                Deactivate.ForEach(x => x.SetActive(false));
            if (Activate != null)
                Activate.ForEach(x => x.SetActive(true));
        }
    }
}