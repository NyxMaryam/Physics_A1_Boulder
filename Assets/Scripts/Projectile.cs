using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string type;
    //Basic Rock

    public int spawnOrder;
    static public int nextSpawnInt = 0;

    public Rigidbody ProjectileRigidbody;

    private void Awake()
    {
        spawnOrder = nextSpawnInt;
        nextSpawnInt++;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Projectile>() != null)
        {
            Projectile hitProj = collision.collider.GetComponent<Projectile>();
            if(hitProj.type == "Basic Rock")
            {
                if(hitProj.spawnOrder < spawnOrder)
                {
                    //Destroy(this);
                }
                else
                {
                    //Projectile.SpawnProjectile(transform.position, transform.rotation, "Two")
                }
            }

        }
    }

    public static Projectile SpawnProjectile(Vector3 positionToSpawnAt, Quaternion rotationToSpawnWith, string projectileTypeToSpawn)
    {
        Projectile newProjectile = null;

        newProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Projectiles/" + projectileTypeToSpawn), positionToSpawnAt, rotationToSpawnWith).GetComponent<Projectile>();

        return newProjectile;
    }
}
