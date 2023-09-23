using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Garage;
using UTanksClient.ECS.Components.Lobby;
using UTanksClient.ECS.Templates.Lobby;
using UTanksClient.ECS.Templates.Shop;

//namespace UTanksClient.ECS
//{
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
        public static DiscountListComponent DiscountList;
    }
//}
