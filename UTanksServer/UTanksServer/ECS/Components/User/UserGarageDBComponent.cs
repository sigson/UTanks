using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components.User
{
    [TypeUid(199367938328007420)]
    public class UserGarageDBComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SelectedEquipmentData selectedEquipment { get; set; }
        public GarageData garage { get; set; }

        public UserGarageDBComponent() { }
        public UserGarageDBComponent(string jsonData)
        {
            UserGarageDBComponent userGarageDBComponent;
            using (StringReader reader = new StringReader(jsonData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                userGarageDBComponent = (UserGarageDBComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(UserGarageDBComponent));
                selectedEquipment = userGarageDBComponent.selectedEquipment;
                garage = userGarageDBComponent.garage;
            }
        }

    }
}
