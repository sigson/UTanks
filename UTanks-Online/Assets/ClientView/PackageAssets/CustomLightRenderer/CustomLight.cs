using UnityEngine;

[ExecuteInEditMode]
public class CustomLight : MonoBehaviour
{
	public enum Kind
	{
		Sphere,
		Tube
	}
	public Kind m_Kind;

	public Color m_Color = Color.white;
	public float m_Intensity = 1.0f;
	public float m_Range = 10.0f;
	public float m_Size = 0.5f;
	public float m_TubeLength = 1.0f;

	public void OnEnable()
	{
		CustomLightSystem.instance.Add (this);
	}

	public void Start()
	{
		CustomLightSystem.instance.Add (this);
	}

	public void OnDisable()
	{
		CustomLightSystem.instance.Remove (this);
	}

	public Color GetLinearColor()
	{
		return new Color(
			Mathf.GammaToLinearSpace(m_Color.r * m_Intensity),
			Mathf.GammaToLinearSpace(m_Color.g * m_Intensity),
			Mathf.GammaToLinearSpace(m_Color.b * m_Intensity),
			1.0f
		);
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, m_Kind==Kind.Tube ? "AreaLight Gizmo" : "PointLight Gizmo", true);
	}
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.1f, 0.7f, 1.0f, 0.6f);
		if (m_Kind == Kind.Tube)
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(m_TubeLength*2, m_Size*2, m_Size*2));
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
		else
		{
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.DrawWireSphere(transform.position, m_Size);
		}
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.DrawWireSphere(transform.position, m_Range);
	}
}
