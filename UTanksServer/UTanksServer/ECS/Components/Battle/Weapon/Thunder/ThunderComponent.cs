﻿using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(-7859561519974477242L)]
	public class ThunderComponent : WeaponComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public float minDamageProperty { get; set; }
        public float maxDamageProperty { get; set; }
        public float reloadTimeProperty { get; set; }
        public float maxDamageDistanceProperty { get; set; }
        public float minDamageDistanceProperty { get; set; }
        public float minDamagePercentProperty { get; set; }
        public float impactProperty { get; set; }
        public float splashImpactProperty { get; set; }
        public float kickbackProperty { get; set; }
        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}