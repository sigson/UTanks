using SecuredSpace.Battle.Tank;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Battle.TankEvents.Shooting
{
    [TypeUid(242632972981530940)]
    public class ShotHPResultEvent : ECSEvent
    {
        public float Damage = 0f;
        public float Heal = 0f;
        public float Critical = 0f;
        public long StruckEntityId = 0;
        static public new long Id { get; set; }
        public override void Execute()
        {
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(this.StruckEntityId, out var entity))
            {
                if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
                {
                    tankManager.ExecuteInstruction(() => {
                        var tUI = tankManager.tankUI.GetComponent<TankUI>();
                        if (tUI.UISwitch.activeInHierarchy)
                            tUI.ShowDamage(Damage);
                    }, "Error show shot hp result");
                }
            }
        }
    }
}
