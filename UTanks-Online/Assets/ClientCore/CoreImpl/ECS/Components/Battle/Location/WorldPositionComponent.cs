﻿using System.Numerics;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace UTanksClient.ECS.Components.Battle.Location
{
    /// <summary>
    /// Bonus position.
    /// </summary>
    [TypeUid(4605414188335188027)]
    public class WorldPositionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WorldPositionComponent() { }
        public WorldPositionComponent(WorldPoint worldPoint)
        {
            WorldPoint = worldPoint;
        }
        
        public WorldPoint WorldPoint { get; set; }
    }
}