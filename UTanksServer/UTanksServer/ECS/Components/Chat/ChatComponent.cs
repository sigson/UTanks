using System.Collections.Concurrent;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1446035193168L)]
    public class ChatComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        ConcurrentDictionary<string, ChatSender> messages = new ConcurrentDictionary<string, ChatSender>();
    }

    [System.Serializable]
    public class ChatSender
    {
        int Rank;
        string Username;
    }
}
