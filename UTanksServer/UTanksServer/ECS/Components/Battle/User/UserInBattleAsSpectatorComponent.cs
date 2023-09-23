//using UTanksServer.Core.Commands;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(4788927540455272293)]
    public class UserInBattleAsSpectatorComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserInBattleAsSpectatorComponent() { }
        public UserInBattleAsSpectatorComponent(long battleId)
        {
            BattleId = battleId;
        }
        
        public long BattleId { get; set; }
    }
}