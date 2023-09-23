﻿using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1485852459997L)]
    public class ModuleGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ModuleGroupComponent() { }
        public ModuleGroupComponent(ECSEntity entity) : base(entity)
        {
        }

        public ModuleGroupComponent(long key) : base(key)
        {
        }
    }
}
