using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(-5670596162316552032L)]
	public class ShaftAimingWorkFinishStateComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public float FinishTimer { get; set; }
		public int ClientTime { get; set; }
	}
}