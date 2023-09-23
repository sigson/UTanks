using System;
using System.Numerics;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;
using UTanksServer.ECS.Types.Battle.Bonus;

namespace UTanksServer.ECS.Components.Battle.Bonus
{
    [TypeUid(-3961778961585441606L)]
    public class BonusRegionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusRegionComponent() { }
        public BonusRegionComponent(BonusType type, string bonusConfig)
        {
            Type = type;
            BonusConfig = bonusConfig;
        }

        public BonusType Type { get; set; }
        public string BonusConfig { get; set; }
        public Vector3S MaxPoint;
        public Vector3S MinPoint;
        private WorldPoint realBasePositon;
        public WorldPoint position {
            get
            {
                if (MaxPoint == null || MinPoint == null)
                    return new WorldPoint();
                var max = new Vector3(MaxPoint.x, MaxPoint.y, realBasePositon.Position.z);
                var min = new Vector3(MinPoint.x, MinPoint.y, realBasePositon.Position.z);
                var randPosition = Vector3.Lerp(min, max, Convert.ToSingle(new Random().NextDouble()));
                return new WorldPoint() { Position = new Vector3S() { x = randPosition.X, y = randPosition.Y, z = randPosition.Z }, Rotation = new Vector3S() };
            }
            set 
            {
                realBasePositon = value;
            } 
        }
    }
}