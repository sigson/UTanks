using System.Numerics;

using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Types.Battle.Weapon {
  public class HitTarget {
    public ECSEntity Entity { get; set; }
    public ECSEntity IncarnationEntity { get; set; }

    public float HitDistance { get; set; }

    public Vector3 HitDirection { get; set; }
    public Vector3 LocalHitPoint { get; set; }
    public Vector3 TargetPosition { get; set; }
  }
}
