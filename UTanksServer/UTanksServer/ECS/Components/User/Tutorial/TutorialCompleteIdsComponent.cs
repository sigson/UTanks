//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using UTanksServer.Core;
//using UTanksServer.Core.Protocol;
//using UTanksServer.ECS.ECSCore;

//namespace UTanksServer.ECS.Components.User.Tutorial
//{
//    [TypeUid(1505286737090)]
//    public class TutorialCompleteIdsComponent : ECSComponent
//    {
//        public TutorialCompleteIdsComponent(List<long> completedIds, Player player)
//        {
//            // ReSharper disable once PossibleNullReferenceException
//            CompletedIds = ((IPEndPoint) player.Connection.Socket.RemoteEndPoint).Address.Equals(IPAddress.Loopback)
//                ? _allIds
//                : new List<long>();
//        }

//        public List<long> CompletedIds { get; set; }
//        public bool TutorialSkipped { get; set; }

//        private readonly List<long> _allIds = new()
//        {
//            -719658163, // firstEntranceTutorial
//            -325846104, // turretControlsTutorial,
//            1662017877, // backHitDamageTutorial,
//            -1969453819, // hullControlsTutorial
//        };
//    }
//}
