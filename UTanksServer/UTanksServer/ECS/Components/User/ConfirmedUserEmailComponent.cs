using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
	[TypeUid(1457515023113L)]
	public class ConfirmedUserEmailComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public ConfirmedUserEmailComponent() { }
		public ConfirmedUserEmailComponent(string Email)
		{
			this.Email = Email;
		}

		public ConfirmedUserEmailComponent(string email, bool subscribed)
		{
			Email = email;
			Subscribed = subscribed;
		}

		public string Email { get; set; }

		public bool Subscribed { get; set; }
	}
}
