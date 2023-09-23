using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.GlobalEntities;

namespace UTanksServer.ECS.Components.Item.Tank
{
    [TypeUid(1436343541876L)]
    public class UpgradeLevelItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UpgradeLevelItemComponent() { }
        public UpgradeLevelItemComponent(long xp)
        {
            //Level = ResourceManager.GetItemLevelByXp(xp);
        }

        public int Level { get; set; }
    }
}
