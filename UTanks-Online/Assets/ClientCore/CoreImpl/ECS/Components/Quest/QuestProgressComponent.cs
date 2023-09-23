using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1476091404409L)]
	public class QuestProgressComponent : ECSComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
		public float PrevValue { get; set; } = 0;

		public float CurrentValue { get; set; } = 0;

		public float TargetValue { get; set; } = 100;

		public bool PrevComplete { get; set; } = true;

		public bool CurrentComplete { get; set; } = true;
	}
}
