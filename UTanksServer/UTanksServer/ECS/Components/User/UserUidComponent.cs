using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
	[TypeUid(-5477085396086342998)]
	public class UserUidComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public UserUidComponent() { }
		public UserUidComponent(string Uid)
		{
			this.Uid = Uid;
		}

		public string Uid { get; set; }
	}
}
