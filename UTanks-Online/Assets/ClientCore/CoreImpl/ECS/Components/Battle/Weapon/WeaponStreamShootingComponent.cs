using System;
using UTanksClient.Core;
//using UTanksClient.Core.Battles.BattleWeapons;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(6803807621463709653L)]
	public class WeaponStreamShootingComponent : TimerComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WeaponStreamShootingComponent() { }
        [OptionalMapped] public long StartShootingTime { get; set; }

        public int Time { get; set; }
	}
}
