using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(-2656312914607478436)]
    public class TankDeadStateComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public TankDeadStateComponent()
        {
            EndTime = DateTime.UtcNow.AddSeconds(3);
        }
        
        public DateTime EndTime { get; set; }
    }
}
