using System;
using System.Reflection;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(196269584046626900)]
    public class GroupComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public GroupComponent() { }

        public GroupComponent(ECSEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            Key = entity.GetId();
        }

        public GroupComponent(long Key)
        {
            this.Key = Key;
        }

        //public long ComponentSerialUID => GetType().GetCustomAttribute<TypeUidAttribute>().Id;
        public long Key { get; set; }
    }
}
