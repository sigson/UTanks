using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.CharacteristicTransformers;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Templates.Battle
{
    [TypeUid(183965749990402300)]
    public class ModificatorTemplate : EntityTemplate
    {
        public static new long Id { get; set; } = 183965749990402300;
        protected override ECSEntity ClientEntityExtendImpl(ECSEntity entity)
        {
            throw new NotImplementedException();
        }

        public static void CreateModificator(ECSEntity playerEntity, float modificatorCoef, params string[] ConfigModificator)
        {
            var configModificators = new List<ConfigObj>();
            ConfigModificator.ForEach(x => configModificators.Add(ConstantService.GetByConfigPath(x)));
            CreateModificatorImpl(playerEntity, modificatorCoef, configModificators.ToArray());
        }

        public static void CreateModificator(ECSEntity playerEntity, float modificatorCoef, params ConfigObj[] ConfigModificator)
        {
            CreateModificatorImpl(playerEntity, modificatorCoef, ConfigModificator);
        }

        private static void CreateModificatorImpl(ECSEntity playerEntity, float modificatorCoef, ConfigObj[] ConfigModificator)
        {
            foreach (var config in ConfigModificator)
            {
                ECSComponent modificatorComponent = null;
                switch (config.Path)
                {
                    case "battle\\modificator\\armor":
                        modificatorComponent = (new ArmorTransformerComponent(false, modificatorCoef));
                        break;
                    case "battle\\modificator\\turbospeed":
                        modificatorComponent = (new NitroTransformerComponent(false, modificatorCoef));
                        break;
                    case "battle\\modificator\\damage":
                        modificatorComponent = (new DamageTransformerComponent(false, modificatorCoef));
                        break;
                    case "battle\\modificator\\healing":
                        modificatorComponent = (new RepairTransformerComponent(false, modificatorCoef));
                        break;
                }
                if (!playerEntity.TryAddComponent((modificatorComponent as ECSComponent).SetGlobalComponentGroup()))
                {
                    var transformer = playerEntity.GetComponent<TimerComponent>((modificatorComponent as ECSComponent).GetId());
                    transformer.TimerReset();
                    transformer.MarkAsChanged();
                }
            }
        }

        public override void InitializeConfigsPath()
        {
            throw new NotImplementedException();
        }
    }
}
