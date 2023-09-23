﻿using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Incarnation
{
    [TypeUid(1491549293967)]
    public class TankIncarnationKillStatisticsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public int Kills { get; set; }
    }
}