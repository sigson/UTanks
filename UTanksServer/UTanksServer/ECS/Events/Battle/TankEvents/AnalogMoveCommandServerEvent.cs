using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Components.Battle.Tank;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(2596682299194665575)]
    public class AnalogMoveCommandServerEvent : ECSEvent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public AnalogMoveCommandServerEvent() { }
        public AnalogMoveCommandServerEvent(Movement movement, MoveControl moveControl, float weaponRotation, float weaponControl)
        {
            Movement = movement;
            MoveControl = moveControl;
            WeaponRotation = weaponRotation;
            WeaponControl = weaponControl;
        }

        [OptionalMapped]
        public Movement Movement { get; set; }

        [OptionalMapped]
        public MoveControl MoveControl { get; set; }

        [OptionalMapped]
        public float WeaponRotation { get; set; }

        [OptionalMapped]
        public float WeaponControl { get; set; }

        public override void Execute()
        {
            //throw new System.NotImplementedException();
        }
    }
}
