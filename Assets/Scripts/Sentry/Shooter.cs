using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject projectile;
    public Transform projectileSpawner;
    EnemyController sentryScript;
    PointAt pointScript;
    public float shootTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        pointScript = GetComponent<PointAt>();

        Transform xform = gameObject.transform;
        do
        {
            if (GetComponentInParent<EnemyController>() != null)
            {
                sentryScript = GetComponentInParent<EnemyController>();
                break;
            }
            xform = xform.parent;
        } while (xform != null);
    }

    void FixedUpdate()
    {
        float distToPlayer = Vector3.SqrMagnitude(pointScript.target.position - transform.position);

        //time ticks if < 10 units away
        if (distToPlayer <= 100f)
        {
            shootTime -= Time.fixedDeltaTime;
            if (shootTime <= 0f)
            {
                //shoot
                Shoot();

                //reset time
                shootTime += 5f;
            }
        }
    }

    void Shoot()
    {
        Instantiate(projectile, projectileSpawner.position, projectileSpawner.rotation);
    }
}
