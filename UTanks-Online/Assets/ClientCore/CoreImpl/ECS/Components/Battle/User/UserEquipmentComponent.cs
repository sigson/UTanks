using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1496906087610)]
    public class UserEquipmentComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserEquipmentComponent() { }
        public UserEquipmentComponent(long weaponId, long hullId)
        {
            WeaponId = weaponId;
            HullId = hullId;
        }
        
        public long WeaponId { get; set; }
        
        public long HullId { get; set; }
    }
}