using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace SecuredSpace.ClientControl.Services
{
    public class RankService : IService
    {
        static public RankService instance => IService.getInstance<RankService>();

        private OrderedDictionary<string, RankRow> rankDB = new OrderedDictionary<string, RankRow>(); //normal, mini, score treshhold, crystal reward

        public RankRow GetRank(string name) => rankDB[name];
        public RankRow GetRank(int index) => rankDB.Get(index);

        public override void InitializeProcess()
        {

        }

        public override void OnDestroyReaction()
        {

        }

        public override void PostInitializeProcess()
        {
            TaskEx.RunAsync(() => {
                while (!ConstantService.instance.Loaded)
                {
                    Task.Delay(20).Wait();
                }
                RankService.instance.ExecuteInstruction(() => {
                    var normalDb = ResourcesService.instance.GameAssets.GetDirectory("ranksconfig\\normal");
                    var miniDb = ResourcesService.instance.GameAssets.GetDirectory("ranksconfig\\mini");
                    var conf = ConstantService.instance.GetByConfigPath("ranksconfig");
                    foreach (var rank in conf.Deserialized["ranksExperiencesConfig"]["ranksExperiences"])
                    {
                        var rankName = rank["name"].ToString();
                        if (rank["scoreThreshold"].ToObject<int>() == -1)
                        {
                            rankDB[rankName] = new RankRow()
                            {
                                name = rankName,
                                scoreThreshold = -1f,
                            };
                            continue;
                        }
                        rankDB[rankName] = new RankRow()
                        {
                            name = rankName,
                            normalSprite = normalDb.GetChildFSObject(rankName).GetContent<Sprite>(),
                            miniSprite = miniDb.GetChildFSObject(rankName).GetContent<Sprite>(),
                            scoreThreshold = rank["scoreThreshold"].ToString().FastFloat(),
                            crystalReward = rank["crystalReward"].ToString().FastFloat()
                        };
                    }
                });
            });
        }
    }

    public sealed class RankRow
    {
        public string name;
        public Sprite normalSprite;
        public Sprite miniSprite;
        public float scoreStartThreshold;
        public float scoreThreshold;
        public float crystalReward;
    }
}