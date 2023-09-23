using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Garage;
using UTanksServer.ECS.Components.Lobby;
using UTanksServer.ECS.Templates.Lobby;
using UTanksServer.ECS.Templates.Shop;

namespace UTanksServer.ECS
{
    public static class InitializeDefaultDataObject
    {
        public static void InitializeDataObjects()
        {
            GlobalGameDataConfig.GarageShop = GarageShopTemplate.CreateComponent();
            GlobalGameDataConfig.DonateShop = DonateShopTemplate.CreateComponent();
            GlobalGameDataConfig.SelectableMap = SelectableMapTemplate.CreateComponent();
        }
    }

    public static class GlobalGameDataConfig
    {
        public static GarageShopComponent GarageShop;
        public static DonateShopComponent DonateShop;
        public static SelectableMapComponent SelectableMap;
        public static DiscountListComponent DiscountList = new DiscountListComponent();
    }
}
