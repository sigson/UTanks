using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.CharacteristicTransformers;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.Components.Battle.Location;
using UTanksServer.ECS.Components.Battle.Round;
using UTanksServer.ECS.Components.Battle.Tank;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy.Battles
{
    [TypeUid(164343269555170140)]
    public class StandartBattlePlayerGDAP : GroupDataAccessPolicy
    {
        public static new long Id = 164343269555170140;
        public StandartBattlePlayerGDAP()
        {
            AvailableComponents = new List<long>() { BattleOwnerComponent.Id, WorldPositionComponent.Id, BattleOwnerComponent.Id, SelfDestructionComponent.Id, TankDeadStateComponent.Id, TankNewStateComponent.Id, TankSpawnStateComponent.Id, TankMovementComponent.Id, TankInBattleComponent.Id, TankActiveStateComponent.Id, StreamWeaponIdleComponent.Id, StreamWeaponWorkingComponent.Id, UserBattleGarageDBComponent.Id,
            SmokyDamageComponent.Id,
                IsisDamageComponent.Id,
                FlamethrowerDamageComponent.Id,
                FreezeDamageComponent.Id,
                ThunderDamageComponent.Id,
                GaussDamageComponent.Id,
                HammerDamageComponent.Id,
                RailgunDamageComponent.Id,
                RicochetDamageComponent.Id,
                ShaftDamageComponent.Id,
                TwinsDamageComponent.Id,
                VulcanDamageComponent.Id,
                HullComponent.Id,
                NitroTransformerComponent.Id,
                RepairTransformerComponent.Id,
                ArmorTransformerComponent.Id,
                DamageTransformerComponent.Id,
                //BattleCreatureStorageComponent.Id,
                RoundUserStatisticsComponent.Id
            };
        }
    }
}
