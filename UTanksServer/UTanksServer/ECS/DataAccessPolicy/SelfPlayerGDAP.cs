using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Energy;
using UTanksServer.ECS.Components.Battle.Health;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.DailyBonus;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy
{
    [TypeUid(226080735867970000)]
    public class SelfPlayerGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 226080735867970000;
        public SelfPlayerGDAP()
        {
            AvailableComponents = new List<long>() {
                UserComponent.Id,
                UsernameComponent.Id,
                UserEmailComponent.Id,
                RegistrationDateComponent.Id,
                UserPrivilegeGroupComponent.Id,
                UserScoreComponent.Id,
                UserRankComponent.Id,
                UserCrystalsComponent.Id,
                UserDailyBonusLastReceivingDateComponent.Id,
                UserGarageDBComponent.Id,
                ChatBanEndTimeComponent.Id,
                UserLocationComponent.Id,
                UserKarmaComponent.Id,
                HealthComponent.Id,
                WeaponEnergyComponent.Id,
                StreamWeaponEnergyComponent.Id,
                WeaponCooldownComponent.Id,
                TankCooldownStorageComponent.Id
            };
        }
    }
}
