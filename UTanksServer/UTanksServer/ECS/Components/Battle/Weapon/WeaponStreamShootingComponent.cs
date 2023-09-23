using System;
using UTanksServer.Core;
//using UTanksServer.Core.Battles.BattleWeapons;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(6803807621463709653L)]
	public class WeaponStreamShootingComponent : TimerComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public WeaponStreamShootingComponent() { }

        public WeaponStreamShootingComponent(StreamWeaponEnergyComponent streamWeaponEnergyComponent)
        {
            onEnd = (entity, weaponStream) =>
            {
                var weaponStreamT = weaponStream as WeaponStreamShootingComponent;
                var UnloadEnergy = streamWeaponEnergyComponent.UnloadEnergyPerSec;
                if (!entity.HasComponent(WeaponCooldownComponent.Id))
                {
                    var energyComponent = entity.GetComponent(WeaponEnergyComponent.Id) as WeaponEnergyComponent;
                    var oldEnergy = energyComponent.Energy;
                    if (energyComponent.Energy - UnloadEnergy >= 0)
                        energyComponent.Energy -= UnloadEnergy;
                    else
                    {
                        energyComponent.Energy = 0;
                        entity.AddComponent(new WeaponCooldownComponent(0.5f));
                    }
                    //entity.AddComponent(new WeaponCooldownComponent(0.25f));
                    if (oldEnergy != energyComponent.Energy)
                    {
                        energyComponent.MarkAsChanged();
                    }
                }
                //TimerStart(0.25f, entity, true, true);
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(0.25f, entity, true, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            ownerEntity = null;
            TimerEnd();
        }

        [OptionalMapped] public long StartShootingTime { get; set; }

        public int Time { get; set; }
	}
}
