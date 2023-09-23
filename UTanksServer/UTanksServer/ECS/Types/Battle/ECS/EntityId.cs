using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.XUETA
{
    [Serializable]
    public class EntityId : CustomValueType<EntityId, Int64>
    {
        private EntityId(long value) : base(value) { }
        public static implicit operator EntityId(long value) { return new EntityId(value); }
        public static implicit operator long(EntityId custom) { return custom._value; }
        public static Type typeLong = typeof(long);
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("", this._value, typeLong);
        //}
    }

    public class CustomValueType<TCustom, TValue> : CachingSerializable
    {
        protected readonly TValue _value;

        public CustomValueType(TValue value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static bool operator <(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return Comparer<TValue>.Default.Compare(a._value, b._value) < 0;
        }

        public static bool operator >(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return !(a < b);
        }

        public static bool operator <=(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return (a < b) || (a == b);
        }

        public static bool operator >=(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return (a > b) || (a == b);
        }

        public static bool operator ==(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return a.Equals((object)b);
        }

        public static bool operator !=(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return !(a == b);
        }

        public static TCustom operator +(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return (dynamic)a._value + b._value;
        }

        public static TCustom operator -(CustomValueType<TCustom, TValue> a, CustomValueType<TCustom, TValue> b)
        {
            return ((dynamic)a._value - b._value);
        }

        protected bool Equals(CustomValueType<TCustom, TValue> other)
        {
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomValueType<TCustom, TValue>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TValue>.Default.GetHashCode(_value);
        }
    }
}
