using System;

namespace UTanksClient.Core.Protocol
{
    /// <summary>
    /// Makes property optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionalMappedAttribute : Attribute
    {
    }
}
