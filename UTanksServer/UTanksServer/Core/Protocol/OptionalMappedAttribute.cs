using System;

namespace UTanksServer.Core.Protocol
{
    /// <summary>
    /// Makes property optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionalMappedAttribute : Attribute
    {
    }
}
