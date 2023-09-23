using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Database.Databases;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Battle.Weapon;
using UTanksServer.ECS.Components.DailyBonus;
using UTanksServer.ECS.Components.ECSComponentsGroup;
using UTanksServer.ECS.Components.User;
using UTanksServer.ECS.DataAccessPolicy;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Network.NetworkEvents.PlayerAuth;
using UTanksServer.Services;

namespace UTanksServer.ECS.Templates.User
{
    [TypeUid(207953786672822900)]
    public class UserTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        private static ConfigObj rankConfig = null;

        public static void ScoreUpdate(ECSEntity playerEntity, int updValue, ConfigObj cachedRankConfig = null)
        {
            var scoreComponent = playerEntity.GetComponent<UserScoreComponent>(UserScoreComponent.Id);
            if (scoreComponent.NextRankScoreThreshold != -1 && scoreComponent.GlobalScore + updValue >= scoreComponent.NextRankScoreThreshold)
            {
                var userRank = playerEntity.GetComponent<UserRankComponent>(UserRankComponent.Id);
                scoreComponent.RankScore = scoreComponent.GlobalScore + scoreComponent.RankScore + updValue - scoreComponent.NextRankScoreThreshold;
                scoreComponent.GlobalScore += updValue;
                userRank.Rank++;    
                
                if(cachedRankConfig != null)
                {
                    rankConfig = cachedRankConfig;
                }
                else
                {
                    if(rankConfig == null)
                        rankConfig = ConstantService.GetByConfigPath("ranksconfig");
                }
                scoreComponent.NextRankScoreThreshold = int.Parse(rankConfig.Deserialized["ranksExperiencesConfig"]["ranksExperiences"][userRank.Rank + 1]["scoreThreshold"].ToString());
                ScoreUpdate(playerEntity, 0, rankConfig);
                userRank.MarkAsChanged();
            }
            else
            {
                scoreComponent.RankScore += updValue;
                scoreComponent.GlobalScore += updValue;
            }
            scoreComponent.MarkAsChanged();
        }

        public ECSEntity CreateEntity(UserRow data, long entityInstanceId)
        {
            var rankConfig = ConstantService.GetByConfigPath("ranksconfig");
            ECSEntity PlayerEntity = new ECSEntity(this, new ECSComponent[] {
                new UserComponent().SetGlobalComponentGroup(),
                new UsernameComponent(data.Username).SetGlobalComponentGroup(),
                new UserEmailComponent(data.Email, data.EmailVerified).SetGlobalComponentGroup(),
                new RegistrationDateComponent(data.RegistrationDate).SetGlobalComponentGroup(),
                //new UserPrivilegeGroupComponent(data.UserGroup).SetGlobalComponentGroup(),
                new UserPrivilegeGroupComponent(data.UserGroup).SetGlobalComponentGroup(),
                new UserRankComponent(data.Rank).SetGlobalComponentGroup(),
                new UserScoreComponent(data.GlobalScore, data.RankScore, int.Parse(rankConfig.Deserialized["ranksExperiencesConfig"]["ranksExperiences"][data.Rank + 1]["scoreThreshold"].ToString())).SetGlobalComponentGroup(),
                new UserCrystalsComponent(data.Crystalls).SetGlobalComponentGroup(),
                new UserDailyBonusLastReceivingDateComponent(data.LastDatetimeGetDailyBonus).SetGlobalComponentGroup(),
                new UserGarageDBComponent(data.GarageJSONData).SetGlobalComponentGroup(),
                //new UserBattleGarageDBComponent().SetGlobalComponentGroup(),
                new ChatBanEndTimeComponent(data.ActiveChatBanEndTime).SetGlobalComponentGroup(),
                new UserLocationComponent(data.UserLocation).SetGlobalComponentGroup(),
                new UserKarmaComponent(data.Karma).SetGlobalComponentGroup(),
                new UserOnlineComponent().SetGlobalComponentGroup(),
                //new CharacteristicTransformerComponent().SetGlobalComponentGroup(),
                //new EffectsAggregatorComponent().SetGlobalComponentGroup(),
                //new ResistanceAggregatorComponent().SetGlobalComponentGroup(),
            });
            //new EquipmentTemplate().SetupEntity(PlayerEntity);
            PlayerEntity.dataAccessPolicies.Add(new SelfPlayerGDAP());
            PlayerEntity.instanceId = entityInstanceId;
            PlayerEntity.TemplateAccessorId.Add(this.GetId());
            return PlayerEntity;
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
