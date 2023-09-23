using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class Node
    {
        private Entity entity;

        public override bool Equals(object o)
        {
            if (ReferenceEquals(this, o))
            {
                return true;
            }
            if (o is Entity)
            {
                return this.entity.Equals(o);
            }
            if ((o == null) || !ReferenceEquals(base.GetType(), o.GetType()))
            {
                return false;
            }
            Node node = (Node)o;
            return !((this.entity == null) ? !ReferenceEquals(node.entity, null) : !this.entity.Equals(node.entity));
        }

        public override int GetHashCode() =>
            (this.entity == null) ? 0 : this.entity.GetHashCode();

        //public T SendEvent<T>(T eventInstance) where T : Event =>
        //    this.Entity.SendEvent<T>(eventInstance);

        public override string ToString() =>
            this.Entity.ToString();

        public virtual Entity Entity
        {
            get =>
                this.entity;
            set =>
                this.entity = value;
        }
    }
}