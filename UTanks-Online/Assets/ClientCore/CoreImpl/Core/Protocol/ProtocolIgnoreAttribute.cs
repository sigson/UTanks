using System;

namespace UTanksClient.Core.Protocol
{
    /// <summary>
    /// Tells encoder and decoder to ignore property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ProtocolIgnoreAttribute : Attribute
    {
    }
}