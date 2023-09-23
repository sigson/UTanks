using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;
using UTanksClient.Services;

namespace UTanksClient.ECS.Components.Garage
{
    [TypeUid(204854765598975360)]
    public class GarageShopComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public GarageShopData garageShop { get; set; }

        public GarageShopComponent() { }
        public GarageShopComponent(string jsonData)
        {
            GarageShopComponent userGarageDBComponent;
            using (StringReader reader = new StringReader(jsonData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                userGarageDBComponent = (GarageShopComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(GarageShopComponent));

                garageShop = userGarageDBComponent.garageShop;
                garageShop.Colormaps = garageShop.Colormaps.OrderBy(x => int.Parse(ConstantService.instance.ConstantDB[x.PathName].Deserialized["crystalsPurchaseUserRankRestriction"]["restrictionValue"].ToString())).ToList();
            }
        }
    }
}
