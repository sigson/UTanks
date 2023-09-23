using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components.Garage
{
    [TypeUid(224227016957314500)]
    public class DonateShopComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public DonateShopData garage { get; set; }

        public DonateShopComponent() { }
        public DonateShopComponent(string jsonData)
        {
            DonateShopComponent userGarageDBComponent;
            using (StringReader reader = new StringReader(jsonData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                userGarageDBComponent = (DonateShopComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(DonateShopComponent));
                garage = userGarageDBComponent.garage;
            }
        }
    }
}
