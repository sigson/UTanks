using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(1464955716416L)]
    public class HammerPelletConeComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public HammerPelletConeComponent() { }
        public HammerPelletConeComponent(float horizontalConeHalfAngle, float verticalConeHalfAngle, int pelletCount)
        {
            HorizontalConeHalfAngle = horizontalConeHalfAngle;
            VerticalConeHalfAngle = verticalConeHalfAngle;
            PelletCount = pelletCount;
        }

        public float HorizontalConeHalfAngle { get; set; }
        public float VerticalConeHalfAngle { get; set; }
        public int PelletCount { get; set; }
    }
}