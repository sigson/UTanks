using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle.Hull
{
    [TypeUid(235757483952985200)]
    public class HullComponent : TankConstructionComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public string hullType { get; set; }
        public float health { get; set; }
        public float weight { get; set; }
        public float damping { get; set; }
        public float speed { get; set; }
        public float acceleration { get; set; }
        public float reverseAcceleration { get; set; }
        public float sideAcceleration { get; set; }
        public float turnSpeed { get; set; }
        public float turnAcceleration { get; set; }
        public float reverseTurnAcceleration { get; set; }
        public float turretTurnSpeedProperty { get; set; }
        public float turretTurnAccelerationProperty { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent baseHullComponent)
        {
            var baseHull = baseHullComponent as BaseHullComponent;
            this.hullType = baseHull.hullType;
            this.health = baseHull.health;
            this.weight = baseHull.weight;
            this.damping = baseHull.damping;
            this.speed = baseHull.speed;
            this.acceleration = baseHull.acceleration;
            this.reverseAcceleration = baseHull.reverseAcceleration;
            this.sideAcceleration = baseHull.sideAcceleration;
            this.turnSpeed = baseHull.turnSpeed;
            this.turnAcceleration = baseHull.turnAcceleration;
            this.reverseTurnAcceleration = baseHull.reverseTurnAcceleration;
            this.turretTurnSpeedProperty = baseHull.turretTurnSpeedProperty;
            this.turretTurnAccelerationProperty = baseHull.turretTurnAccelerationProperty;
            return this;
        }
    }
}
