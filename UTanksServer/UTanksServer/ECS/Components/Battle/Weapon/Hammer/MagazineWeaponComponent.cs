using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon.Hammer
{
    [TypeUid(4355651182908057733L)]
    public class MagazineWeaponComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MagazineWeaponComponent() { }
        public MagazineWeaponComponent(int maxCartridgeCount, float reloadMagazineTimePerSec)
        {
            MaxCartridgeCount = maxCartridgeCount;
            ReloadMagazineTimePerSec = reloadMagazineTimePerSec;
        }

        public int MaxCartridgeCount { get; set; }

        public float ReloadMagazineTimePerSec { get; set; }
    }
}
