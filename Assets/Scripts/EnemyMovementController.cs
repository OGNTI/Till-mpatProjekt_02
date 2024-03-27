using UnityEngine;
using UnityEngine.AI;


[SelectionBase]
public class EnemyMovementController : MonoBehaviour
{
    EnemyViewController viewController;
    EnemyAttackController attackController;

    NavMeshAgent agent;

    float roamTimer = 0;
    float timeBetweenRoam = 4;
    float forgedTimeBetweenRoam;

    [SerializeField] int speed = 5;
    [SerializeField] float turnSpeed = 5;

    bool targetFound = false;
    bool placeHolder = false;

    float searchTimer = 0;
    float searchRomeTime = 2;
    float thing;
    float giveUpSeachTime = 10;

    int SpreadAngle = 75;
    int SpreadRange = 3;

    string state;
    string[] states = { "roam", "follow", "attack", "search", "spreadOut" };
    Vector3 targetLastKnownPosition;
    Vector3 dirFromPlayerToSelf;

    GameObject player;

    bool targetInRange = false;
    bool allyBlocking = false;

    void Awake()
    {
        viewController = gameObject.GetComponent<EnemyViewController>();
        attackController = gameObject.GetComponent<EnemyAttackController>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;

        state = states[0];
        forgedTimeBetweenRoam = timeBetweenRoam;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        dirFromPlayerToSelf = (transform.position - player.transform.position).normalized;

        if (state == states[0])
        {
            Debug.Log("What nice weather.");
            roamTimer += Time.deltaTime;
            if (roamTimer > forgedTimeBetweenRoam)
            {
                agent.SetDestination(RandomNavMeshLocationInRange(Random.Range(4f, 18f)));
                roamTimer = 0;
                forgedTimeBetweenRoam = UnityEngine.Random.Range(timeBetweenRoam - (timeBetweenRoam / 3), timeBetweenRoam + (timeBetweenRoam / 3));
            }
        }
        else if (state == states[1])
        {
            Debug.Log("Moving on Target.");
            if (targetInRange == false)
            {
                if (agent.SetDestination(player.transform.position + (dirFromPlayerToSelf * viewController.attackRange)))
                {
                    state = states[2];
                }
            }
            else state = states[2];
        }
        else if (state == states[2])
        {
            Debug.Log("Eliminate.");
            if (targetFound == false) OnPlayerLost();
            else BeginAttack();
        }
        else if (state == states[3])
        {
            Debug.Log("where did he go?");

            if (!placeHolder)
            {
                agent.SetDestination(targetLastKnownPosition);
                thing = 0;
                searchTimer = 0;

                // Check if we've reached the destination
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            placeHolder = true;
                        }
                    }
                }
            }
            else
            {
                searchTimer += Time.deltaTime;
                if (searchTimer > giveUpSeachTime)
                {
                    state = states[0];
                    searchTimer = 0;
                }
                else if (searchTimer > thing)
                {
                    agent.SetDestination(RandomNavMeshLocationInRange(Random.Range(1f, 3f)));
                    thing += searchRomeTime;
                }
            }
        }
        else if (state == states[4])
        {
            if (allyBlocking)
            {
                Debug.Log("Oh no my friend is in the way, gotta move.");
                Vector3 dir = Vector3.zero;
                Vector3 newDestination = transform.position;

                int a = Random.Range(1, 3);
                if (a == 1)
                {
                    dir = viewController.DirFromAngle(-SpreadAngle);
                }
                else if (a == 2)
                {
                    dir = viewController.DirFromAngle(SpreadAngle);
                }

                newDestination = dir * SpreadRange + transform.position;
                NavMesh.SamplePosition(newDestination, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
                agent.SetDestination(hit.position);

                allyBlocking = false;
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

    public Vector3 RandomNavMeshLocationInRange(float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        Vector3 randomPosition = randomDirection + transform.position;
        Vector3 finalPosition = Vector3.zero;

        NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, range, NavMesh.AllAreas);
        finalPosition = hit.position;
        return finalPosition;
    }

    void OnPlayerFound()
    {
        StopMovement();
        state = states[1];
        Debug.Log("Found");
        targetFound = true;
    }

    void OnPlayerLost()
    {
        state = states[3];
        Debug.Log("Lost");
        targetFound = false;
        targetLastKnownPosition = player.transform.position;
        placeHolder = false;
        attackController.SendMessage("CannotAttack");
    }

    void OnTargetInRange()
    {
        StopMovement();
        targetInRange = true;
    }

    void OnTargetOutsideRange()
    {
        state = states[1];
        targetInRange = false;
        attackController.SendMessage("CannotAttack");
    }

    void BeginAttack()
    {
        allyBlocking = false;
        attackController.SendMessage("CanAttack");
    }

    void FaceTarget(GameObject target)
    {
        agent.updateRotation = false;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position);
    }

    void SpreadOut()
    {
        state = states[4];
        allyBlocking = true;
    }
}
