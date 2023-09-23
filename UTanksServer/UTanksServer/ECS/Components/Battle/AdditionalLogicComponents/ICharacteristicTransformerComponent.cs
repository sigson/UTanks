using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Hull;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.AdditionalLogicComponents
{
    [TypeUid(195412052829804540)]
    public interface ICharacteristicTransformerComponent
    {
        public Action<ECSEntity, ECSComponent> TransformAction { get; set; }
        public Action<ECSEntity, ECSComponent> UndoTransformAction { get; set; }
        public WeaponComponent InfluenceWeaponStorage { get; set; }
        public TankConstructionComponent InfluenceHullStorage { get; set; }
        public bool stableTransformer { get; set; } //dont remove on dead or exit from battle
    }
}
