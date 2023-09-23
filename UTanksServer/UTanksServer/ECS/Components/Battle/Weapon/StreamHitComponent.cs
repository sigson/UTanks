using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(-6274985110858845212L)]
    public class StreamHitComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //[OptionalMapped]
        //public HitTarget TankHit { get; set; }

        //[OptionalMapped]
        //public StaticHit StaticHit { get; set; }
    }
}
