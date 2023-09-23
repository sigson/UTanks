﻿using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(1437983715951L)]
    public class ShaftAimingImpactComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ShaftAimingImpactComponent() { }
        public ShaftAimingImpactComponent(float maxImpactForce)
        {
            MaxImpactForce = maxImpactForce;
        }

        public float MaxImpactForce { get; set; }
    }
}