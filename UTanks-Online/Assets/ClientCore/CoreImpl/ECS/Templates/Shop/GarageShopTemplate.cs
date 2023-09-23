using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Garage;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Services;

namespace UTanksClient.ECS.Templates.Shop
{
    [TypeUid(194044951111624000)]
    public class GarageShopTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            return null;
        }

        public static GarageShopComponent CreateComponent()
        {
            #region old
            //ConstantService.instance.GetByConfigPath
            //GarageShopComponent bufComponent;
            //using (StringReader reader = new StringReader())
            //{
            //    JsonTextReader jreader = new JsonTextReader(reader);
            //    bufComponent = (GarageShopComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(GarageShopComponent));
            //}
            //ECSEntity PlayerEntity = new ECSEntity(this, new ECSComponent[] {
            //    bufComponent
            //});
            ////PlayerEntity.dataAccessPolicies.Add(new SelfPlayerGDAP());
            //PlayerEntity.instanceId = Guid.NewGuid().GuidToLong();
            //PlayerEntity.TemplateAccessorId.Add(this.GetId());
            #endregion
            return new GarageShopComponent(File.ReadAllText(GlobalProgramState.ConfigDir + "garageshop.json"));
        }

        public override void InitializeConfigsPath()
        {
            
        }
    }
}
