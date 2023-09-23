﻿using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Item.Tank
{
    [TypeUid(1436338996992L)]
    public class ExperienceItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ExperienceItemComponent() { }
        public ExperienceItemComponent(long experience)
        {
            Experience = experience;
        }

        public long Experience { get; set; }
    }
}
