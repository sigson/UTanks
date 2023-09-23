using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Battle.Tank
{
    public class IManagableAnchor : IManagable
    {
        public T ownerManager<T>() where T : IEntityManager
        {
            return (T)ownerManagerSpace;
        }
    }

}