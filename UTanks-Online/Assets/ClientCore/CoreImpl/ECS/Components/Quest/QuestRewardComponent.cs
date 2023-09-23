﻿using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1493196614850L)]
    public class QuestRewardComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public QuestRewardComponent() { }
        public QuestRewardComponent(Dictionary<ECSEntity, int> Reward)
        {
            this.Reward = Reward;
        }

        public Dictionary<ECSEntity, int> Reward { get; set; }
    }
}
