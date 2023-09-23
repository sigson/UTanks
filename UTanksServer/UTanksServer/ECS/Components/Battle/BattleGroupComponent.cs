﻿using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(1140613249019529884)]
    public class BattleGroupComponent : ECSComponentGroup
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleGroupComponent() { }
    }
}