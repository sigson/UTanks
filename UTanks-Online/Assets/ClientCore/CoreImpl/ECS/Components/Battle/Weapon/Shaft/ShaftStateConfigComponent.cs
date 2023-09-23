using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(635950079224407790L)]
    public class ShaftStateConfigComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ShaftStateConfigComponent() { }
        public ShaftStateConfigComponent(float waitingToActivationTransitionTimeSec, float activationToWorkingTransitionTimeSec, float finishToIdleTransitionTimeSec)
        {
            WaitingToActivationTransitionTimeSec = waitingToActivationTransitionTimeSec;
            ActivationToWorkingTransitionTimeSec = activationToWorkingTransitionTimeSec;
            FinishToIdleTransitionTimeSec = finishToIdleTransitionTimeSec;
        }

        public float WaitingToActivationTransitionTimeSec { get; set; }
        public float ActivationToWorkingTransitionTimeSec { get; set; }
        public float FinishToIdleTransitionTimeSec { get; set; }
    }
}