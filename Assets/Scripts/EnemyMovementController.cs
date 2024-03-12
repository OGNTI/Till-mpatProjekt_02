using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[SelectionBase]
public class EnemyMovementController : MonoBehaviour
{
    NavMeshAgent agent;

    float roamTimer = 0;
    float timeBetweenRoam = 4;
    float forgedTimeBetweenRoam;

    [SerializeField] float attackRange = 15;

    [SerializeField] int speed = 5;
    [SerializeField] float turnSpeed = 5;

    bool targetFound = false;

    float searchTimer = 0;
    float giveUpSeach = 10;

    string state;
    string[] states = { "roam", "follow", "attack", "search" };
    Vector3 targetLastKnownPosition;

    GameObject player;

    void Awake()
    {
        state = states[0];
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;
        forgedTimeBetweenRoam = timeBetweenRoam;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (state == states[0])
        {
            roamTimer += Time.deltaTime;
            if (roamTimer > forgedTimeBetweenRoam)
            {
                agent.SetDestination(RandomNavMeshLocationInRange(UnityEngine.Random.Range(1, 9)));
                roamTimer = 0;
                forgedTimeBetweenRoam = UnityEngine.Random.Range(timeBetweenRoam - (timeBetweenRoam / 3), timeBetweenRoam + (timeBetweenRoam / 3));
            }
        }
        else if (state == states[1])
        {
            if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
            {
                FaceTarget(player);
                Vector3 dirFromPlayerToSelf = transform.position - player.transform.position;
                agent.SetDestination(dirFromPlayerToSelf * attackRange);
                state = states[2];
            }
            else
            {
                state = states[2];
            }
        }
        else if (state == states[2])
        {
            Debug.Log("die die die");

            if (targetFound == false)
            {
                targetLastKnownPosition = player.transform.position;
                state = states[3];
            }
        }
        else if (state == states[3])
        {
            searchTimer += Time.deltaTime;
            
            Debug.Log("where tf did he go?");
            agent.SetDestination(targetLastKnownPosition);
            
            if (searchTimer > giveUpSeach)
            {
                state = states[0];
            }
        }

        if (targetFound == true)
        {
            FaceTarget(player);
        }
        else if (targetFound == false)
        {
            agent.updateRotation = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(player.transform.position, player.transform.position + (transform.position - player.transform.position));
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

    void OnPlayerFound()
    {
        state = states[1];
        Debug.Log("Found");
        targetFound = true;
    }

    void OnPlayerLost()
    {
        state = states[3];
        Debug.Log("Lost");
        targetFound = false;
    }

    void FaceTarget(GameObject target)
    {
        agent.updateRotation = false;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
