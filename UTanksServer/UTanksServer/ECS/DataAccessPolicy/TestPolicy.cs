using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.Notification;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.DataAccessPolicy
{
    [TypeUid(176312779278210020)]
    public class TestPolicy : GroupDataAccessPolicy
    {
        public static new long Id = 176312779278210020;
        public TestPolicy()
        {
            AvailableComponents = new List<long>() { UserCrystalsComponent.Id, ServerNotificationMessageComponent.Id };
        }
    }
}
