using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(-6549017400741137637)]
    public class BattleOwnerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        public ECSEntity Battle;
        public long BattleInstanceId;

        public BattleOwnerComponent() { }
        public BattleOwnerComponent(ECSEntity battle) 
        {
            this.Battle = battle;
            this.BattleInstanceId = battle.instanceId;
        }
    }
}