using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(223782033584306100)]
    public class DamageResistComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public long DamageComponentId;
        public int WeaponResistPercent { get; set; }
        public override void OnAdded(ECSEntity entity)
        {
            //base.OnAdded(entity);
            var resistanceAggregator = entity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
            resistanceAggregator.resistanceAggregator[DamageComponentId] = (damageComponent, effectComponent, changableComponent, chgValue) =>
            {
                if (damageComponent.GetId() == DamageComponentId)
                {
                    if (changableComponent.GetId() == HealthComponent.Id)
                    {
                        var health = changableComponent as HealthComponent;
                        health.CurrentHealth -= chgValue * (100 - WeaponResistPercent) / 100;
                    }
                }
            };
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            //var resistanceAggregator = entity.GetComponent(ResistanceAggregatorComponent.Id) as ResistanceAggregatorComponent;
            //if(!resistanceAggregator.resistanceAggregator.TryRemove(DamageComponent.GetId(), out _))
            //{
            //    Logger.LogError("resist remove error");
            //}
        }
    }
}
