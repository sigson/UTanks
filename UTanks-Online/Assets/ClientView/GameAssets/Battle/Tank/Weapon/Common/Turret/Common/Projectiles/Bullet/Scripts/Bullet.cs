using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int maxRicoshets;
    public float perRicAlpha;
    public float Speed;
    public string tagToDamage;
    public float lifetime;
    public float mass = 0.06f;
    public GameObject explosion;
    public GameObject reflectBurst;
    public float deformationFactor;
    public float deformationSize;
    public Vector3 velocity;
    public Vector3 gravity;
    public int rics;
    RaycastHit rhit;
    bool destroying = false;
    void Start()
    {
        rics = Random.Range(0, maxRicoshets + 1);
        //networkView.SendMessage("SetRics", rics);
        velocity = transform.up * Speed;
        Destroy(gameObject, lifetime);
        float magn = Vector3.Magnitude(velocity);
        if (Physics.Raycast(transform.position, velocity, out rhit, magn))
        {
            if (rhit.transform.tag == tagToDamage)
            {
                //Deformator defrm = rhit.transform.gameObject.GetComponent<Deformator>();
                //if (defrm != null) defrm.DeformMe(rhit.point, velocity, deformationFactor * magn, deformationSize);
                Instantiate(explosion, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                velocity = new Vector3(0, 0, 0);
                destroying = true;
                rhit.transform.gameObject.BroadcastMessage("Damage", damage * magn);
            }
            if (rics > 0)
            {
                if (rhit.rigidbody != null)
                {
                    rhit.rigidbody.velocity += velocity * mass / rhit.rigidbody.mass;
                }
                velocity = Vector3.Reflect(velocity, rhit.normal) * perRicAlpha;
                Instantiate(reflectBurst, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                rics--;
            }
            else if (rics == 0)
            {
                if (rhit.rigidbody != null)
                {
                    rhit.rigidbody.velocity += velocity * mass / rhit.rigidbody.mass;
                }
                Instantiate(explosion, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                velocity = new Vector3(0, 0, 0);
                destroying = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (destroying)
        {
            Destroy(gameObject);
            return;
        }
        //Debug.DrawRay(transform.position, rigidbody.velocity*Time.deltaTime*10f, Color.blue);
        float magn = Vector3.Magnitude(velocity);
        if (Physics.Raycast(transform.position, velocity, out rhit, magn))
        {
            if (rhit.transform.tag == tagToDamage)
            {
                //Deformator defrm = rhit.transform.gameObject.GetComponent<Deformator>();
                //if (defrm != null) defrm.DeformMe(rhit.point, velocity, deformationFactor * magn, deformationSize);
                Instantiate(explosion, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                velocity = new Vector3(0, 0, 0);
                destroying = true;
                rhit.transform.gameObject.BroadcastMessage("Damage", damage * magn);
            }
            else if (rics > 0)
            {
                if (rhit.rigidbody != null)
                {
                    rhit.rigidbody.velocity += velocity * mass / rhit.rigidbody.mass;
                }
                velocity = Vector3.Reflect(velocity, rhit.normal) * perRicAlpha;
                Instantiate(reflectBurst, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                rics--;
            }
            else if (rics == 0)
            {
                if (rhit.rigidbody != null)
                {
                    rhit.rigidbody.velocity += velocity * mass / rhit.rigidbody.mass;
                }
                Instantiate(explosion, rhit.point, Quaternion.LookRotation(new Vector3(rhit.normal.x + 90, rhit.normal.y, rhit.normal.z)));
                transform.position = rhit.point;
                velocity = new Vector3(0, 0, 0);
                destroying = true;
            }
        }
        velocity += gravity;
        transform.position += velocity;
    }

    void SetRics(int srics)
    {
        rics = srics;
    }
}