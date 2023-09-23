using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;

namespace UTanksClient.ECS.Components.User
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

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            //if (!ClientInit.CheckEntityIsPlayer(entity))
            //    return;
            //ClientInit.uiManager.ExecuteInstruction((object Obj) =>
            //{
            //    if (this.garage != null && this.selectedEquipment != null)
            //        ClientInit.uiManager.GarageUI.GetComponent<GarageUIHandler>().GarageSetup(this);
            //}, null);
        }
    }
}
