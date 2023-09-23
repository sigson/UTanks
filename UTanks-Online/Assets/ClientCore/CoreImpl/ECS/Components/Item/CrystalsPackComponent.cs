using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1507791399668)]
	public class CrystalsPackComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public long Amount { get; set; } = 0;

		public long Bonus { get; set; } = 0;
	}
}
