using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Settings
{
    public class IAnchor : IManagable
    {
        private bool registered = false;
        public bool objectEnabled = true;
        protected override void AwakeImpl()
        {
            if (!registered)
            {
                ClientInitService.instance.anchorHandler.RegisterAnchor(this);
                registered = true;
                UpdateObject();
            }
        }

        public void Start()
        {
            StartImpl();
        }

        protected override void OnDestroy()
        {
            ClientInitService.instance.anchorHandler.UnregisterAnchor(this);
            base.OnDestroy();
        }

        public void DirectRegisterObject()
        {
            if (!registered)
            {
                ClientInitService.instance.anchorHandler.RegisterAnchor(this);
                registered = true;
                UpdateObject();
            }
        }

        protected virtual void StartImpl()
        {

        }

        public void UpdateObject()
        {
            if(this == null)
            {
                ClientInitService.instance.anchorHandler.UnregisterAnchor(this);
                return;
            }
            if (this.gameObject.activeInHierarchy)
                StartCoroutine(UpdateObjectImpl());
            else
                UpdateObjectImpl().MoveNext();
        }

        protected virtual IEnumerator UpdateObjectImpl()
        {
            yield return new WaitForEndOfFrame();
        }
    }

}