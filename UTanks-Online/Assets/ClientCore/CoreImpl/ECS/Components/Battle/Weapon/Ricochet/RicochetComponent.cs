using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(2115384895595656627L)]
	public class RicochetComponent : WeaponComponent
	{
		static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public override TankConstructionComponent UpdateComponent(TankConstructionComponent tankComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}