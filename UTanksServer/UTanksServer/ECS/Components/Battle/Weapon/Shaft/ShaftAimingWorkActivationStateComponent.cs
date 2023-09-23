using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(8631717637564140236L)]
	public class ShaftAimingWorkActivationStateComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public float ActivationTimer { get; set; }
		public int ClientTime { get; set; }
	}
}