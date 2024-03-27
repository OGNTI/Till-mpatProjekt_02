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

    [SerializeField] float inaccuracy = 3;

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

            float inaccuracyAngle1 = Random.Range(-inaccuracy, inaccuracy + 1);
            float inaccuracyAngle2 = Random.Range(-inaccuracy, inaccuracy + 1);
            Quaternion inaccuracyRotation = transform.rotation * Quaternion.AngleAxis(inaccuracyAngle1, Vector3.up) * Quaternion.AngleAxis(inaccuracyAngle2, Vector3.right); //Make bullets less accurate

            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.transform.position, inaccuracyRotation);
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
