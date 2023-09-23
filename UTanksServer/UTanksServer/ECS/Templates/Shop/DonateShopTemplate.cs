using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Garage;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Templates.Shop
{
    [TypeUid(244896212409591840)]
    public class DonateShopTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 207953786672822900;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public static DonateShopComponent CreateComponent()
        {
            #region del
            //ConstantService.GetByConfigPath
            //DonateShopComponent bufComponent;
            //using (StringReader reader = new StringReader())
            //{
            //    JsonTextReader jreader = new JsonTextReader(reader);
            //    bufComponent = (DonateShopComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(DonateShopComponent));
            //}
            //ECSEntity PlayerEntity = new ECSEntity(this, new ECSComponent[] {
            //    bufComponent
            //});
            ////PlayerEntity.dataAccessPolicies.Add(new SelfPlayerGDAP());
            //PlayerEntity.instanceId = Guid.NewGuid().GuidToLong();
            //PlayerEntity.TemplateAccessorId.Add(this.GetId());
            #endregion
            return new DonateShopComponent(File.ReadAllText(GlobalProgramState.ConfigDir + "donateshop.json"));
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
