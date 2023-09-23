using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.GlobalEntities;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1499175516647)]
    public class KillsEquipmentStatisticsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //public Dictionary<Entity, long> HullStatistics { get; set; } = new Dictionary<Entity, long>()
        //{
        //    { Weapons.GlobalItems.Flamethrower, 0 },
        //    { Weapons.GlobalItems.Freeze, 0 },
        //    { Weapons.GlobalItems.Hammer, 0 },
        //    { Weapons.GlobalItems.Isis, 0 },
        //    { Weapons.GlobalItems.Railgun, 0 },
        //    { Weapons.GlobalItems.Ricochet, 0 },
        //    { Weapons.GlobalItems.Shaft, 0 },
        //    { Weapons.GlobalItems.Smoky, 0 },
        //    { Weapons.GlobalItems.Thunder, 0 },
        //    { Weapons.GlobalItems.Twins, 0 },
        //    { Weapons.GlobalItems.Vulcan, 0 }
        //};

        //public Dictionary<Entity, long> TurretStatistics { get; set; } = new Dictionary<Entity, long>()
        //{
        //    { Hulls.GlobalItems.Wasp, 0 },
        //    { Hulls.GlobalItems.Hornet, 0 },
        //    { Hulls.GlobalItems.Hunter, 0 },
        //    { Hulls.GlobalItems.Viking, 0 },
        //    { Hulls.GlobalItems.Titan, 0 },
        //    { Hulls.GlobalItems.Dictator, 0 },
        //    { Hulls.GlobalItems.Mammoth, 0 }
        //};
    }
}
