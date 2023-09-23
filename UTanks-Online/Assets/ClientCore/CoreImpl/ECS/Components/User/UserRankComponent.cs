using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(-1413405458500615976)]
    public class UserRankComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserRankComponent() { }
        public UserRankComponent(int Rank)
        {
            this.Rank = Rank;
            
        }

        public int Rank { get; set; }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            
        }

    }
}
