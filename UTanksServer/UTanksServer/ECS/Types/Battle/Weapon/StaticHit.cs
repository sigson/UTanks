using System.Numerics;

namespace UTanksServer.ECS.Types.Battle.Weapon {
  public class StaticHit {
    public Vector3 Normal { get; set; }
    public Vector3 Position { get; set; }

    public override string ToString() => $"Position: {Position}, Normal: {Normal}";
  }
}
