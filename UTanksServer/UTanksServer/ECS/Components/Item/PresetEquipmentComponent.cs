//using System.Collections.Generic;
//using System.Linq;
//using UTanksServer.Core;
//using UTanksServer.Core.Protocol;
//using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.GlobalEntities;

//namespace UTanksServer.ECS.Components
//{
//	[TypeUid(1502886877871L)]
//	public class PresetEquipmentComponent : ECSComponent
//    {
//        private Player Player;

//        public PresetEquipmentComponent(Player player, Entity preset)
//        {
//            Player = player;
//            Preset = preset;

//            Weapons.Items weaponList = (Weapons.Items)player.UserItems[typeof(Weapons)];
//            Hulls.Items hullList = (Hulls.Items)player.UserItems[typeof(Hulls)];
//            WeaponSkins.Items weaponSkinList = (WeaponSkins.Items)player.UserItems[typeof(WeaponSkins)];
//            HullSkins.Items hullSkinList = (HullSkins.Items)player.UserItems[typeof(HullSkins)];
//            Shells.Items shellList = (Shells.Items)player.UserItems[typeof(Shells)];
//            ModuleSlots.Items moduleSlotList = (ModuleSlots.Items)player.UserItems[typeof(ModuleSlots)];
//            Covers.Items coverList = (Covers.Items)Player.UserItems[typeof(Covers)];
//            Paints.Items paintList = (Paints.Items) Player.UserItems[typeof(Paints)];
//            Graffiti.Items graffitiList = (Graffiti.Items) Player.UserItems[typeof(Graffiti)];

//            bool restore = player.RestorablePreset != null;

//            WeaponItem = restore ? FindItem(player.RestorablePreset.WeaponItem, weaponList, player) : weaponList.Smoky;
//            HullItem = restore ? FindItem(player.RestorablePreset.HullItem, hullList, player) : hullList.Hunter;

//            Weapon = restore ? player.RestorablePreset?.Weapon : Weapons.GlobalItems.Smoky;
//            WeaponId = Weapon.EntityId;
//            Hull = restore ? player.RestorablePreset?.Hull : Hulls.GlobalItems.Hunter;
//            HullId = Hull.EntityId;

//            WeaponPaint = restore ? FindItem(player.RestorablePreset.WeaponPaint, coverList, player) : coverList.None;
//            TankPaint = restore ? FindItem(player.RestorablePreset.TankPaint, paintList, player) : paintList.Green;

//            Graffiti = restore ? FindItem(player.RestorablePreset.Graffiti, graffitiList, player) : graffitiList.Logo;

//            WeaponSkins = new Dictionary<Entity, Entity>
//            {
//                { weaponList.Flamethrower, weaponSkinList.FlamethrowerM0 },
//                { weaponList.Freeze, weaponSkinList.FreezeM0 },
//                { weaponList.Hammer, weaponSkinList.HammerM0 },
//                { weaponList.Isis, weaponSkinList.IsisM0 },
//                { weaponList.Railgun, weaponSkinList.RailgunM0 },
//                { weaponList.Ricochet, weaponSkinList.RicochetM0 },
//                { weaponList.Shaft, weaponSkinList.ShaftM0 },
//                { weaponList.Smoky, weaponSkinList.SmokyM0 },
//                { weaponList.Thunder, weaponSkinList.ThunderM0 },
//                { weaponList.Twins, weaponSkinList.TwinsM0 },
//                { weaponList.Vulcan, weaponSkinList.VulcanM0 }
//            };

//            HullSkins = new Dictionary<Entity, Entity>
//            {
//                { hullList.Dictator, hullSkinList.DictatorM0 },
//                { hullList.Hornet, hullSkinList.HornetM0 },
//                { hullList.Hunter, hullSkinList.HunterM0 },
//                { hullList.Mammoth, hullSkinList.MammothM0 },
//                { hullList.Titan, hullSkinList.TitanM0 },
//                { hullList.Viking, hullSkinList.VikingM0 },
//                { hullList.Wasp, hullSkinList.WaspM0 }
//            };

//            WeaponShells = new Dictionary<Entity, Entity>
//            {
//                { weaponList.Flamethrower, shellList.FlamethrowerOrange },
//                { weaponList.Freeze, shellList.FreezeSkyblue },
//                { weaponList.Hammer, shellList.HammerStandard },
//                { weaponList.Isis, shellList.IsisStandard },
//                { weaponList.Railgun, shellList.RailgunPaleblue },
//                { weaponList.Ricochet, shellList.RicochetAurulent },
//                { weaponList.Shaft, shellList.ShaftStandard },
//                { weaponList.Smoky, shellList.SmokyStandard },
//                { weaponList.Thunder, shellList.ThunderStandard },
//                { weaponList.Twins, shellList.TwinsBlue },
//                { weaponList.Vulcan, shellList.VulcanStandard }
//            };

//            Modules = new Dictionary<Entity, Entity>
//            {
//                { moduleSlotList.Slot1, null },
//                { moduleSlotList.Slot2, null },
//                { moduleSlotList.Slot3, null },
//                { moduleSlotList.Slot4, null },
//                { moduleSlotList.Slot5, null },
//                { moduleSlotList.Slot6, null }
//            };

//            if (restore)
//            {
//                WeaponSkins[WeaponItem] = FindItem(player.RestorablePreset.WeaponSkins[player.RestorablePreset.WeaponItem],
//                    weaponSkinList, player);
//                HullSkins[HullItem] = FindItem(player.RestorablePreset.HullSkins[player.RestorablePreset.HullItem],
//                    hullSkinList, player);
//                WeaponShells[WeaponItem] = FindItem(player.RestorablePreset.WeaponShells[player.RestorablePreset.WeaponItem],
//                    shellList, player);
//            }
//        }

//        [ProtocolIgnore] public Entity Weapon { get; set; }
//        public long weaponId { get; set; }
//        [ProtocolIgnore]
//        public long WeaponId
//        {
//            set
//            {
//                weaponId = value;
//                if (Preset.TryRemoveComponent<PresetEquipmentComponent>())
//                    Preset.AddComponent(this);
//            }
//        }

//        [ProtocolIgnore] public Entity Hull { get; set; }
//        public long hullId { get; set; }

//        [ProtocolIgnore]
//        public long HullId
//        {
//            set
//            {
//                hullId = value;
//                if (Preset.TryRemoveComponent<PresetEquipmentComponent>())
//                    Preset.AddComponent(this);
//            }
//        }

//        [ProtocolIgnore] public Entity Preset { get; set; }

//        [ProtocolIgnore] public Entity WeaponItem { get; set; }
//        [ProtocolIgnore] public Entity HullItem { get; set; }

//        [ProtocolIgnore] public Entity WeaponPaint { get; set; }
//        [ProtocolIgnore] public Entity TankPaint { get; set; }

//        [ProtocolIgnore] public Entity Graffiti { get; set; }
//        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponSkins { get; set; }
//        [ProtocolIgnore] public Dictionary<Entity, Entity> HullSkins { get; set; }

//        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponShells { get; set; }
//        [ProtocolIgnore] public Dictionary<Entity, Entity> Modules { get; set; }

//        private static Entity FindItem(Entity restorableItem, ItemList selfItemList, Player player)
//        {
//            return selfItemList.GetAllItems()
//                .SingleOrDefault(t =>
//                    t.TemplateAccessor.ConfigPath == restorableItem.TemplateAccessor.ConfigPath);
//        }
//    }
//}
