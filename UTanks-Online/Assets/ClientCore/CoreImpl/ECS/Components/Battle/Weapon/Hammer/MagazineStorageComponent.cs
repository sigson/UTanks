using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon.Hammer
{
    [TypeUid(2388237143993578319L)]
    public class MagazineStorageComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public MagazineStorageComponent() { }
        public MagazineStorageComponent(int currentCartridgeCount)
        {
            CurrentCartridgeCount = currentCartridgeCount;
        }

        public int CurrentCartridgeCount { get; set; }
    }
}
