using Newtonsoft.Json;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Round
{
    [TypeUid(-5556650973238726161L)]
    public class RoundRestartingStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        [System.NonSerialized]
        [JsonIgnore]
        public Dictionary<long, float> rewardStorage = new Dictionary<long, float>();

        public RoundRestartingStateComponent() { }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                ClientInitService.instance.LockInput = true;
                UIService.instance.battleUIHandler.StatsWindow.SetActive(true);

            }, null);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                ClientInitService.instance.LockInput = false;
                UIService.instance.battleUIHandler.StatsWindow.SetActive(false);

            }, null);
        }

        public float TimeRestart;
    }
}