//using UTanksClient.Core;
//using UTanksClient.Core.Configuration;
//using UTanksClient.Core.Protocol;
//using UTanksClient.ECS.ECSCore;
//using UTanksClient.ECS.GlobalEntities;
//using UTanksClient.ECS.ServerComponents;

//namespace UTanksClient.ECS.Components.User
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
