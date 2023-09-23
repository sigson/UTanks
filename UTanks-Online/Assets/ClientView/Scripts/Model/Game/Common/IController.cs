using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IController : IManagable
    {
        //public abstract void Build();
        //public abstract void ResetControlled();
        protected virtual T AppendControllerToTarget<T>(GameObject target) where T : IController
        {
            return target.AddComponent<T>();
        }

        protected static IController AppendControllerToTarget(GameObject target, System.Type controller)
        {
            return (IController)target.AddComponent(controller);
        }
    }
}