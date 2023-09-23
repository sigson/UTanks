using System;

namespace UTanksServer.Core.Protocol
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SerialVersionUIDAttribute : Attribute
    {
        private SerialVersionUIDAttribute() { }

        public SerialVersionUIDAttribute(Int64 Id)
        {
            this.Id = Id;
        }

        public readonly Int64 Id;
    }
}