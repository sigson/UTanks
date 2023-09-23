using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.Special
{
    public class UpdateUIObject : MonoBehaviour
    {
        private GameObject updateGameObject;
        private Action<GameObject> updateAction = null;
        private Action<GameObject> preUpdateAction = null;
        public bool UpdateOnEnable = false;

        public void Init(GameObject updategameObject, Action<GameObject> updateaction, Action<GameObject> preupdateAction)
        {
            updateGameObject = updategameObject;
            updateAction = updateaction;
            preUpdateAction = preupdateAction;
        }

        public void OnEnable()
        {
            if (UpdateOnEnable)
                UpdateObject();
        }

        public void UpdateObject()
        {
            if (updateGameObject == null)
                updateGameObject = this.gameObject;
            if (preUpdateAction != null)
                preUpdateAction(updateGameObject);
            else
            {
                preUpdateAction = (gameObject) =>
                {
                    //ContentLayoutRefreshMarker.RefreshContentFitter(gameObject.transform.GetComponent<RectTransform>());
                    //var updateUI = gameObject.GetComponent<UpdateUIObject>();
                    //try
                    //{
                    //    var vertical = gameObject.GetComponent<VerticalLayoutGroup>();
                    //    if (vertical != null)
                    //        LayoutRebuilder.ForceRebuildLayoutImmediate(vertical.transform.GetComponent<RectTransform>());
                    //}
                    //catch { }
                    
                    //try
                    //{
                    //    var horizontal = gameObject.GetComponent<HorizontalLayoutGroup>();
                    //    if (horizontal != null)
                    //        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontal.transform.GetComponent<RectTransform>());
                    //}
                    //catch { }

                    //try
                    //{
                    //    var horizontal = gameObject.GetComponent<ContentSizeFitter>();
                    //    if (horizontal != null)
                    //        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontal.transform.GetComponent<RectTransform>());
                    //}
                    //catch { }

                };
                updateAction = (gameObject) =>
                {
                    //try
                    //{
                    //    var vertical = gameObject.GetComponent<VerticalLayoutGroup>();
                    //    if (vertical != null)
                    //        vertical.childForceExpandWidth = false;
                    //}
                    //catch { }

                    //try
                    //{
                    //    var horizontal = gameObject.GetComponent<HorizontalLayoutGroup>();
                    //    if (horizontal != null)
                    //        horizontal.childForceExpandHeight = false;
                    //}
                    //catch { }

                    //try
                    //{
                    //    var horizontal = gameObject.GetComponent<ContentSizeFitter>();
                    //    if (horizontal != null)
                    //        horizontal.enabled = true;
                    //}
                    //catch { }

                };
                preUpdateAction(updateGameObject);
            }
            if (updateAction != null && this.gameObject.activeInHierarchy)
            {
                StartCoroutine("UpdateCoroutine");
            }
            if(!this.gameObject.activeInHierarchy)
            {
                updateAction(updateGameObject);
            }
        }

        IEnumerator UpdateCoroutine()
        {
            yield return new WaitForSeconds(0.001f);
            if(updateGameObject != null)
                updateAction(updateGameObject);
            else
                updateAction(this.gameObject);
        }
    }

}