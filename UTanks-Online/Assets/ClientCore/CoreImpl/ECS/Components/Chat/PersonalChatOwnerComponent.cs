using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1513067769958)]
    public class PersonalChatOwnerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public List<ECSEntity> ChatsIs { get; set; } = new List<ECSEntity>();
    }
}
