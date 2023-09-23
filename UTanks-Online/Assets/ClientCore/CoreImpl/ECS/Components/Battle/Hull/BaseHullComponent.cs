using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(222707156977181120)]
    public class BaseHullComponent : TankConstructionComponent
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

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent weaponComponent)
        {
            return null;
        }
    }
}
