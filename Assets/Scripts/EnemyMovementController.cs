using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class EnemyMovementController : MonoBehaviour
{
    NavMeshAgent agent;

    float timer = 0;
    float timeBetweenRoam = 4;
    float placeHolder;

    [SerializeField] int speed = 5;



    void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;
        placeHolder = timeBetweenRoam;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > placeHolder)
        {
            agent.SetDestination(RandomNavMeshLocationInRange(UnityEngine.Random.Range(0, 10)));
            timer = 0;
            placeHolder = UnityEngine.Random.Range(timeBetweenRoam - (timeBetweenRoam / 3), timeBetweenRoam + (timeBetweenRoam / 3));
        }


    }


    public Vector3 RandomNavMeshLocationInRange(float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
