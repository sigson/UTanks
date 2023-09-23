using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IComponentManager : IECSObjectManager<ECSComponent>
    {
        public ECSComponent ManagerComponent
        {
            get
            {
                return this.ManagerECSObject;
            }
            set
            {
                this.ManagerECSObject = value;
            }
        }

        public long ManagerComponentId => this.ManagerECSObjectId;
    }
}