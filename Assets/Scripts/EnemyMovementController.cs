using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[SelectionBase]
public class EnemyMovementController : MonoBehaviour
{
    NavMeshAgent agent;

    float timer = 0;
    float timeBetweenRoam = 4;
    float placeHolder;

    [SerializeField] int speed = 5;

    string state = "roam";
    string[] states = { "roam", "follow", "attack" };

    GameObject player;

    void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;
        placeHolder = timeBetweenRoam;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (state == states[0])
        {
            timer += Time.deltaTime;
            if (timer > placeHolder)
            {
                agent.SetDestination(RandomNavMeshLocationInRange(UnityEngine.Random.Range(1, 9)));
                timer = 0;
                placeHolder = UnityEngine.Random.Range(timeBetweenRoam - (timeBetweenRoam / 3), timeBetweenRoam + (timeBetweenRoam / 3));
            }
        }
        else if (state == states[1])
        {
            Vector3 DirFromPlayerToSelf = (transform.position - player.transform.position).normalized;
            agent.SetDestination(player.transform.position * 20);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(player.transform.position + (transform.position - player.transform.position).normalized, player.transform.position);
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

    }

    void OnPlayerLost()
    {
        state = states[0];
        Debug.Log("Lost");
    }
}
