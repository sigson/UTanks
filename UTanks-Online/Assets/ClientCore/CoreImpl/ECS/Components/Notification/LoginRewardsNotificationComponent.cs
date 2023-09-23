//using System.Collections.Generic;
//using System.Linq;
//using UTanksClient.Core;
//using UTanksClient.Core.Protocol;
//using UTanksClient.ECS.ECSCore;
//using UTanksClient.ECS.GlobalEntities;
//using UTanksClient.ECS.Types;

//namespace UTanksClient.ECS.Components.Notification
//{
//    [TypeUid(1523423182023L)]
//    public class LoginRewardsNotificationComponent : ECSComponent
//    {
//        public LoginRewardsNotificationComponent(Player player)
//        {
//            foreach (LoginRewardItem reward in AllReward.Where(r => r.Day == player.Data.RecruitRewardDay))
//            {
//                Reward.Add(reward.MarketItemEntity, reward.Amount);
//                player.SaveNewMarketItem(player.EntityList.Single(e => e.EntityId == reward.MarketItemEntity),
//                    reward.Amount);
//            }

//            CurrentDay = player.Data.RecruitRewardDay;
//        }

//        public Dictionary<long, int> Reward { get; set; } = new();

//        public List<LoginRewardItem> AllReward { get; set; } = new() {
//            new(1, ExtraItems.GlobalItems.Xcrystal.EntityId, 49),
//            new(2, ExtraItems.GlobalItems.Crystal.EntityId, 500),
//            new(3, ExtraItems.GlobalItems.Crystal.EntityId, 750),
//            new(4, ExtraItems.GlobalItems.Xcrystal.EntityId, 99),
//            new(5, ExtraItems.GlobalItems.Crystal.EntityId, 850),
//            new(6, Containers.GlobalItems.Cardssilverdonut.EntityId, 1),
//            new(7, ExtraItems.GlobalItems.Crystal.EntityId, 1500),
//            new(8, ExtraItems.GlobalItems.Crystal.EntityId, 1800), new LoginRewardItem(8, Paints.GlobalItems.Champion.EntityId, 1),
//            new(9, ExtraItems.GlobalItems.Premiumboost.EntityId, 5),
//            new(10, ExtraItems.GlobalItems.Xcrystal.EntityId, 150), new(10, Paints.GlobalItems.Coal.EntityId, 1),
//            new(11, ExtraItems.GlobalItems.Crystal.EntityId, 3500),
//            new(12, Containers.GlobalItems.Cardsgolddonut.EntityId, 1),
//            new(13, Containers.GlobalItems.Steel.EntityId, 1),
//            new(14, ExtraItems.GlobalItems.Premiumboost.EntityId, 7),
//            new(15, Containers.GlobalItems.Xt.EntityId, 1),
//        };

//        public int CurrentDay { get; set; }
//    }
//}
