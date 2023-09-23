using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.ECS.ECSCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class TypeUidAttribute : Attribute
    {
        public long Id { get; set; }

        public TypeUidAttribute(long id)
        {
            Id = id;
        }
    }

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    //public sealed class OnChangeComponentCallbackAttribute : Attribute
    //{
    //    public List<Action> Actions { get; set; }

    //    public OnChangeComponentCallbackAttribute(List<Action> actions)
    //    {
    //        Actions = actions;
    //    }
    //}

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ServerOnlyDataAttribute : Attribute
    {
        public ServerOnlyDataAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProtocolOptionalAttribute : Attribute
    {
        public ProtocolOptionalAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProtocolNameAttribute : Attribute
    {
        public string Name { get; }

        public ProtocolNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceAttribute() { }
    }

}
