using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IManagable : ProxyBehaviour
    {
        public IEntityManager ownerManagerSpace = null;
        public long instanceId = Guid.NewGuid().GuidToLong();
        public List<UnityEngine.Object> ChildTemp = new List<UnityEngine.Object>();

        protected virtual void AwakeImpl()
        {
            if(ownerManagerSpace != null)
            {
                ownerManagerSpace.AddManagable(this);
            }
        }

        protected virtual void Awake()
        {
            AwakeImpl();
        }

        protected virtual void OnDestroyImpl()
        {
            if (ownerManagerSpace != null)
            {
                ownerManagerSpace.RemoveManagable(this);
            }
        }

        protected virtual void OnDestroy()
        {
            ChildTemp.ForEach(x => Destroy(x));
            OnDestroyImpl();
        }
    }
}