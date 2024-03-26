using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    EnemyMovementController movementController;
    EnemyViewController viewController;

    [SerializeField] GameObject bulletPrefab;
    GameObject gunBarrel;

    float fireTimer = 0;
    [SerializeField] float fireRate = 1;

    bool targetInRange = false;

    void Awake()
    {
        movementController = gameObject.GetComponent<EnemyMovementController>();
        viewController = gameObject.GetComponent<EnemyViewController>();
        gunBarrel = gameObject.transform.GetChild(2).gameObject;
    }

    void Update()
    {
        if (targetInRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer > fireRate)
        {
            fireTimer = 0;
            
            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.transform.position, transform.rotation);
            Destroy(bullet, 2f);
        }
    }

    void CanAttack()
    {
        targetInRange = true;
    }

    void CannotAttack()
    {
        targetInRange = false;
    }
}
