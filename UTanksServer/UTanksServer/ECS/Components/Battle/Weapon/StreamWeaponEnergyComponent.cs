using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(1438077278464L)]
	public class StreamWeaponEnergyComponent : TimerComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public StreamWeaponEnergyComponent() { }
        public StreamWeaponEnergyComponent(float reloadEnergyPerSec, float unloadEnergyPerSec)
        {
            ReloadEnergyPerSec = reloadEnergyPerSec;
            UnloadEnergyPerSec = unloadEnergyPerSec;
            onEnd = (entity, streamWeaponEnergyComp) =>
            {
                if(!entity.HasComponent(WeaponStreamShootingComponent.Id) && entity.HasComponent(TankInBattleComponent.Id))
                {
                    var energyComponent = entity.GetComponent(WeaponEnergyComponent.Id) as WeaponEnergyComponent;
                    var oldEnergy = energyComponent.Energy;
                    if (energyComponent.Energy + ReloadEnergyPerSec >= energyComponent.MaxEnergy)
                        energyComponent.Energy = energyComponent.MaxEnergy;
                    else
                        energyComponent.Energy += ReloadEnergyPerSec;
                    if (oldEnergy != energyComponent.Energy)
                    {
                        energyComponent.MarkAsChanged();
                    }
                }
                //TimerStart(0.25f, entity, true);
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

        public float ReloadEnergyPerSec { get; set; }

		public float UnloadEnergyPerSec { get; set; }
	}
}