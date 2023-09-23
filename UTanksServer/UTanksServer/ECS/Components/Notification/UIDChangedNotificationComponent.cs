using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1475754429807L)]
    public class UIDChangedNotificationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        // warning: Alternativa logic: oldUserUID = newUserUID 
        public UIDChangedNotificationComponent() { }
        public UIDChangedNotificationComponent(string oldUserUID)
        {
            OldUserUID = oldUserUID;
        }

        public string OldUserUID { get; set; }
    }
}
