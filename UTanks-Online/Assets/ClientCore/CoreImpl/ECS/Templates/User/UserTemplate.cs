using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.DailyBonus;
using UTanksClient.ECS.Components.ECSComponentsGroup;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Network.NetworkEvents.PlayerAuth;
using UTanksClient.Services;

namespace UTanksClient.ECS.Templates.User
{
    [TypeUid(207953786672822900)]
    public class UserTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }

        public override void InitializeConfigsPath()
        {
            
        }
    }
}
