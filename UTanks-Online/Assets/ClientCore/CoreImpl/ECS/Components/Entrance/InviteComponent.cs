using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Entrance
{
    [TypeUid(1439808320725L)]
	public class InviteComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public InviteComponent() { }
		public InviteComponent(bool showScreenOnEntrance, string inviteCode)
        {
			ShowScreenOnEntrance = showScreenOnEntrance;
			InviteCode = inviteCode;
        }

        public bool ShowScreenOnEntrance { get; set; }
		[OptionalMapped]
		public string InviteCode { get; set; }
	}
}
