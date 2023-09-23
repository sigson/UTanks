using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksClient.ECS.Types
{
    public class TankPart : CachingSerializable
    {
        public string PathName { get; set; }
        public List<Skin> Skins { get; set; }
        public int Grade { get; set; }
    }
    public class Turret : TankPart
    {
        
    }

    public class Hull : TankPart
    {

    }

    public class Skin : CachingSerializable
    {
        public string SkinPathName { get; set; }
        public bool Equiped { get; set; }
    }

    public class Colormap : CachingSerializable
    {
        public string PathName { get; set; }
    }

    public class Module : CachingSerializable
    {
        public string PathName { get; set; }
        public bool Equiped { get; set; }
    }

    public class VisualizableEquipment : CachingSerializable
    {
        public List<Turret> Turrets { get; set; }
        public List<Hull> Hulls { get; set; }
        public List<Colormap> Colormaps { get; set; }
    }

    public class SelectedEquipmentData : VisualizableEquipment
    {
        public List<Module> Modules { get; set; }
    }

    public class Supply : CachingSerializable
    {
        public string PathName { get; set; }
        public int Count { get; set; }
    }

    public class ModuleWithTimeExpiration : CachingSerializable
    {
        public string PathName { get; set; }
        public long TimeOfExpiration { get; set; }
    }

    public class OtherObject : CachingSerializable
    {
        public string PathName { get; set; }
        public int Count { get; set; }
        public long TimeOfExpiration { get; set; }
    }

    public class GarageData : VisualizableEquipment
    {
        public List<Module> Modules { get; set; }
        public List<Supply> Supplies { get; set; }
        public List<OtherObject> OtherObjects { get; set; }
        public List<ModuleWithTimeExpiration> ModuleWithTimeExpiration { get; set; }
    }

    public class WeaponKit : CachingSerializable
    {
        public string PathName { get; set; }
    }

    public class GarageShopData : GarageData
    {
        public List<WeaponKit> WeaponKits { get; set; }
    }

    public class DonateShopData : CachingSerializable
    {
        public List<OtherObject> DonateItem { get; set; }
    }
}
