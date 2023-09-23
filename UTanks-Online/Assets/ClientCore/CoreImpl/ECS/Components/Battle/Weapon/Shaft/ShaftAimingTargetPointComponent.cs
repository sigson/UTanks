using System.Numerics;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(8445798616771064825L)]
	public class ShaftAimingTargetPointComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public bool IsInsideTankPart { get; set; }

		[OptionalMapped]
		public Vector3 Point { get; set; }
	}
}