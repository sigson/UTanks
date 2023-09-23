using System;
using System.Runtime.CompilerServices;

namespace UTanksServer.Core.Protocol
{
    /// <summary>
    /// Sets fixed fixed property position.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolFixedAttribute : Attribute
    {
        public ProtocolFixedAttribute([CallerLineNumber]int position = 0)
        {
            this.Position = position;
        }

        public int Position { get; }
    }
}