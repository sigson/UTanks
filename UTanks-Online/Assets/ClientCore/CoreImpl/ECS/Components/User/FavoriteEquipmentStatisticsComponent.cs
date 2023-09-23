//using System.Collections.Generic;
//using UTanksClient.Core.Protocol;
//using UTanksClient.ECS.ECSCore;
//using UTanksClient.ECS.GlobalEntities;

//namespace UTanksClient.ECS.Components
//{
//    [TypeUid(1522236020112)]
//    public class FavoriteEquipmentStatisticsComponent : ECSComponent
//    {
//        public Dictionary<Entity, long> HullStatistics { get; set; } = new Dictionary<Entity, long>()
//        {
//            { Weapons.GlobalItems.Flamethrower, 0 },
//            { Weapons.GlobalItems.Freeze, 0 },
//            { Weapons.GlobalItems.Hammer, 0 },
//            { Weapons.GlobalItems.Isis, 0 },
//            { Weapons.GlobalItems.Railgun, 0 },
//            { Weapons.GlobalItems.Ricochet, 0 },
//            { Weapons.GlobalItems.Shaft, 0 },
//            { Weapons.GlobalItems.Smoky, 0 },
//            { Weapons.GlobalItems.Thunder, 0 },
//            { Weapons.GlobalItems.Twins, 0 },
//            { Weapons.GlobalItems.Vulcan, 0 }
//        };

//        public Dictionary<ECSEntity, long> TurretStatistics { get; set; } = new Dictionary<ECSEntity, long>()
//        {
//            { Hulls.GlobalItems.Wasp, 0 },
//            { Hulls.GlobalItems.Hornet, 0 },
//            { Hulls.GlobalItems.Hunter, 0 },
//            { Hulls.GlobalItems.Viking, 0 },
//            { Hulls.GlobalItems.Titan, 0 },
//            { Hulls.GlobalItems.Dictator, 0 },
//            { Hulls.GlobalItems.Mammoth, 0 }
//        };
//    }
//}
