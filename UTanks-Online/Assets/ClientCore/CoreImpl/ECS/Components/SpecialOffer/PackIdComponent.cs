using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(636185325862880582)]
    public class PackIdComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public PackIdComponent() { }
        public PackIdComponent(long Id)
        {
            this.PackId = Id;
        }

        public long PackId { get; set; }
    }
}
