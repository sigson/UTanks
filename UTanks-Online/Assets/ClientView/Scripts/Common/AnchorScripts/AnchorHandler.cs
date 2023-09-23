using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Core.Logging;

namespace SecuredSpace.Settings
{
    public class AnchorHandler
    {
        public Dictionary<Type, List<IAnchor>> AnchorDB = new Dictionary<Type, List<IAnchor>>();

        public void RegisterAnchor(IAnchor anchor)
        {
            List<IAnchor> anchorDBElement = null;
            if (AnchorDB.TryGetValue(anchor.GetType(), out anchorDBElement))
            {
                if(!anchorDBElement.Contains(anchor))
                {
                    anchorDBElement.Add(anchor);
                }
            }
            else
            {
                anchorDBElement = new List<IAnchor>();
                anchorDBElement.Add(anchor);
                AnchorDB.Add(anchor.GetType(), anchorDBElement);
            }
        }

        public void UnregisterAnchor(IAnchor anchor)
        {
            List<IAnchor> anchorDBElement = null;
            if (AnchorDB.TryGetValue(anchor.GetType(), out anchorDBElement))
            {
                try
                {
                    anchorDBElement.Remove(anchor);
                }
                catch
                {
                    ULogger.Log("try to remove not registered anchor");
                }
            }
            else
            {
                ULogger.Log("try to remove not registered anchor");
            }
        }

        public void UpdateAnchors(Type anchorType, Func<IAnchor, bool> UpdateCondition = null)
        {
            List<IAnchor> anchorDBElement = null;
            if (AnchorDB.TryGetValue(anchorType, out anchorDBElement))
            {
                for (int i = 0; i < anchorDBElement.Count; i++)
                {
                    var anchor = anchorDBElement[i];
                    if (UpdateCondition != null && UpdateCondition(anchor))
                    {
                        int countCache = anchorDBElement.Count;
                        anchor.UpdateObject();
                        if (countCache != anchorDBElement.Count)
                            i--;
                    }
                }
            }
        }

        public void UpdateAnchors<T>(Func<IAnchor, bool> UpdateCondition = null) => UpdateAnchors(typeof(T), UpdateCondition);
    }

}