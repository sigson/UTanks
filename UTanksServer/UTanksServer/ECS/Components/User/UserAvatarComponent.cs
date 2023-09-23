//using UTanksServer.Core;
//using UTanksServer.Core.Configuration;
//using UTanksServer.Core.Protocol;
//using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.GlobalEntities;
//using UTanksServer.ECS.ServerComponents;

//namespace UTanksServer.ECS.Components.User
//{
//    [TypeUid(1545809085571)]
//    public class UserAvatarComponent : ECSComponent
//    {
//        public UserAvatarComponent(long avatarMarketId, Player player)
//        {
//            string configPath = (player.GetEntityById(avatarMarketId) ?? Avatars.GlobalItems.Tankist).TemplateAccessor
//                .ConfigPath;
//            Id = Config.GetComponent<AvatarItemComponent>(configPath).Id;
//        }

//        public string Id { get; set; }
//    }
//}
