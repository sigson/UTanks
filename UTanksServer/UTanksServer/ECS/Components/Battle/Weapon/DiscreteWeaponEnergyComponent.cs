using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(1438077188268L)]
    public class DiscreteWeaponEnergyComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public float reloadTime = 1f;
        public DiscreteWeaponEnergyComponent() { }
        public DiscreteWeaponEnergyComponent(float reloadEnergyPerSec, float unloadEnergyPerShot, bool reloadNoWaitFullEnergyConsumption)
        {
            ReloadEnergyPerSec = reloadEnergyPerSec;
            UnloadEnergyPerShot = unloadEnergyPerShot;
            ReloadNoWaitFullEnergyConsumption = reloadNoWaitFullEnergyConsumption;
            onEnd = (entity, discreteWeaponEnergyComp) =>
            {
                var energyComponent = entity.GetComponent(WeaponEnergyComponent.Id) as WeaponEnergyComponent;
                var oldEnergy = energyComponent.Energy;
                var discreteWeaponEnergy = discreteWeaponEnergyComp as DiscreteWeaponEnergyComponent;
                if (reloadNoWaitFullEnergyConsumption || discreteWeaponEnergy.ReloadingAfterZeroEnergy)
                {  
                    if (energyComponent.Energy + ReloadEnergyPerSec >= energyComponent.MaxEnergy)
                    {
                        energyComponent.Energy = energyComponent.MaxEnergy;
                        discreteWeaponEnergy.ReloadingAfterZeroEnergy = false;
                    }             
                    else
                        energyComponent.Energy += ReloadEnergyPerSec;
                    if(oldEnergy != energyComponent.Energy)
                    {
                        energyComponent.MarkAsChanged();
                    }
                }
                else if(energyComponent.Energy == 0)
                {
                    discreteWeaponEnergy.ReloadingAfterZeroEnergy = true;
                }
                //TimerStart(1f, entity, true);
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            TimerStart(reloadTime, entity, true, true);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            ownerEntity = null;
            TimerEnd();
        }
        public bool ReloadNoWaitFullEnergyConsumption { get; set; }//true example - ricochet, shaft, false example - hammer

        public float ReloadEnergyPerSec { get; set; }

        public float UnloadEnergyPerShot { get; set; }

        public bool ReloadingAfterZeroEnergy;
    }
}