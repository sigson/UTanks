using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(-7296556060693494496L)]
    public class BattleECSConfigurationComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //string is a command
        Dictionary<string, List<GroupDataAccessPolicy>> commandsGDAP = new Dictionary<string, List<GroupDataAccessPolicy>>();
        Dictionary<string, List<ECSComponent>> CharacteristicTransformers = new Dictionary<string, List<ECSComponent>>();
        Dictionary<string, List<ECSComponent>> Effects = new Dictionary<string, List<ECSComponent>>();
        Dictionary<string, List<ECSComponent>> Resists = new Dictionary<string, List<ECSComponent>>();
        Dictionary<string, List<ECSComponent>> disabledCharacteristicTransformers = new Dictionary<string, List<ECSComponent>>();
        Dictionary<string, List<ECSComponent>> disabledEffects = new Dictionary<string, List<ECSComponent>>();
        Dictionary<string, List<ECSComponent>> disabledResists = new Dictionary<string, List<ECSComponent>>();
    }
}