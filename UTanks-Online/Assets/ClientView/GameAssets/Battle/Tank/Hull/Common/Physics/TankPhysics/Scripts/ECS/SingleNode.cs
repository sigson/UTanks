using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class SingleNode<T> : Node where T : Component
    {
        public T component;
    }
}