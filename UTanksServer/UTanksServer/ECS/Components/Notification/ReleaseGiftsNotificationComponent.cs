//using System.Collections.Generic;
//using System.Linq;
//using UTanksServer.Core;
//using UTanksServer.Core.Protocol;
//using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.GlobalEntities;

//namespace UTanksServer.ECS.Components.Notification
//{
//    [TypeUid(636564551794907676L)]
//    public class ReleaseGiftsNotificationComponent : ECSComponent
//    {
//        public ReleaseGiftsNotificationComponent(Player player)
//        {
//            foreach ((long rewardId, int amount) in Reward)
//            {
//                Entity rewardMarketItem = player.EntityList.Single(e => e.EntityId == rewardId);
//                player.SaveNewMarketItem(rewardMarketItem, amount);
//            }

//            player.Data.ReceivedReleaseReward = true;
//        }

//        public Dictionary<long, int> Reward { get; set; } = new()
//        {
//            { Paints.GlobalItems.Beginning.EntityId, 1 },
//            { Containers.GlobalItems.Cardssilverdonut.EntityId, 10 },
//            { ExtraItems.GlobalItems.Xcrystal.EntityId, 350 }
//        };
//    }
//}
