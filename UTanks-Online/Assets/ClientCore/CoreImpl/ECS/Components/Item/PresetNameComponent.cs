using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1493974995307L)]
	public class PresetNameComponent : ECSComponent
	{
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public PresetNameComponent() { }
        public PresetNameComponent(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
	}
}
