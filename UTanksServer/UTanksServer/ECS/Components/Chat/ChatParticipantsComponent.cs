using System;
using System.Collections.Generic;
using System.Linq;
using UTanksServer.Core;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(636437655901996504L)]
    public class ChatParticipantsComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ChatParticipantsComponent() { }
        public ChatParticipantsComponent(List<ECSEntity> users)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public ChatParticipantsComponent(params ECSEntity[] users)
        {
            Users = users.ToList();
        }

        public List<ECSEntity> Users { get; set; }

        //public IEnumerable<Player> GetPlayers()
        //{
        //    return Enumerable.Select(Users, user => Server.Instance.FindPlayerByUid(user.EntityId));
        //}
    }
}
