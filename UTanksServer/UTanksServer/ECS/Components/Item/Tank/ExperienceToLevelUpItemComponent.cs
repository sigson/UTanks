//using System.Collections.Generic;
//using System.Linq;
//using UTanksServer.Core.Configuration;
//using UTanksServer.Core.Protocol;
//using UTanksServer.ECS.ECSCore;
//using UTanksServer.ECS.ServerComponents.Experience;

//namespace UTanksServer.ECS.Components.Item.Tank
//{
//    [TypeUid(1438924983080L)]
//    public class ExperienceToLevelUpItemComponent : ECSComponent
//    {
//        public ExperienceToLevelUpItemComponent(long xp)
//        {
//            List<int> experiencePerRank = new List<int>(0);
//            experiencePerRank.AddRange(Config.GetComponent<UpgradeLevelsComponent>("garage").LevelsExperiences
//                .ToList());

//            int index = experiencePerRank.IndexOf(experiencePerRank.LastOrDefault(x => x <= xp));
//            FinalLevelExperience = experiencePerRank[index + (index >= 8 ? 0 : 1)];
//            InitLevelExperience =
//                experiencePerRank[experiencePerRank.IndexOf(experiencePerRank.LastOrDefault(x => x <= xp)) +
//                                  (index >= 8 ? 0 : 1)];
//            RemainingExperience = (int) (FinalLevelExperience - xp);
//        }

//        public int RemainingExperience { get; set; }
//        public int InitLevelExperience { get; set; }
//        public int FinalLevelExperience { get; set; }
//    }
//}
