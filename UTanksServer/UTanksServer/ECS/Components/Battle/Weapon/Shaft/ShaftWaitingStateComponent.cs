using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(6541712051864507498L)]
	public class ShaftWaitingStateComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public float WaitingTimer { get; set; }
		public int Time { get; set; }
	}
}