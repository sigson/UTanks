﻿using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(5853281840501157320)]
    public class TankMovableComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
    }
}
