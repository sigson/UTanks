using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Lobby;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Templates.Lobby
{
    [TypeUid(187244467689847840)]
    public class SelectableMapTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }

        public static SelectableMapComponent CreateComponent()
        {
            #region old
            //ConstantService.instance.GetByConfigPath
            //SelectableMapComponent bufComponent;
            //using (StringReader reader = new StringReader())
            //{
            //    JsonTextReader jreader = new JsonTextReader(reader);
            //    bufComponent = (SelectableMapComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(SelectableMapComponent));
            //}
            //ECSEntity PlayerEntity = new ECSEntity(this, new ECSComponent[] {
            //    bufComponent
            //});
            ////PlayerEntity.dataAccessPolicies.Add(new SelfPlayerGDAP());
            //PlayerEntity.instanceId = Guid.NewGuid().GuidToLong();
            //PlayerEntity.TemplateAccessorId.Add(this.GetId());
            #endregion
            return new SelectableMapComponent(File.ReadAllText(GlobalProgramState.ConfigDir + "selectablemapdb.json"));
        }

        public override void InitializeConfigsPath()
        {
            
        }
    }
}
