using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    public float viewRange;
    public float viewAngle;
    public float attackRange;

    GameObject player;
    GameObject viewTarget;

    EnemyMovementController movementController;
    
    GameObject eyesParent;
    Renderer[] eyes;
    Color standardEyeColor;

    bool playerInViewRange = false;
    bool playerInAttackRange = false;

    Vector3 dirToPlayer;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementController = gameObject.GetComponent<EnemyMovementController>();
        eyesParent = gameObject.transform.GetChild(1).gameObject;
        eyes = eyesParent.GetComponentsInChildren<Renderer>();
        standardEyeColor = eyes[0].material.color;
        Mathf.Clamp(viewAngle, 0, 360);
    }

    void Update()
    {
        dirToPlayer = player.transform.position - transform.position;

        GameObject oldViewTarget = viewTarget;

        //is player in viewRange and viewAngle
        if (Vector3.Distance(transform.position, player.transform.position) < viewRange &&
            Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            Physics.SphereCast(transform.position, 0.3f, dirToPlayer, out RaycastHit hit, viewRange, Physics.DefaultRaycastLayers); //is sight blocked or not
            Debug.DrawLine(transform.position, hit.point, Color.green);

            if (hit.collider != null) viewTarget = hit.collider.gameObject;
            else viewTarget = null;

            if (viewTarget.tag == "Player")
            {
                if (oldViewTarget != viewTarget) PlayerFound();

                if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                {
                    if (playerInAttackRange == false) InAttackRange();
                }
                else OutOfAttackRange();
            }
            else if (oldViewTarget != viewTarget && viewTarget.tag == "Enemy") AllyBlockingSight();
            else if (oldViewTarget != viewTarget && viewTarget.tag != "Player") PlayerLost();
        }
        else if (playerInViewRange == true) PlayerLost();
    }

    public Vector3 DirFromAngle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void PlayerFound()
    {
        foreach (Renderer r in eyes)
        {
            r.material.color = Color.red;
        }
        movementController.SendMessage("OnPlayerFound");
        playerInViewRange = true;

        Debug.Log("hsfsh");
    }

    void PlayerLost()
    {
        foreach (Renderer r in eyes)
        {
            r.material.color = standardEyeColor;
        }
        movementController.SendMessage("OnPlayerLost");
        playerInViewRange = false;
    }

    void InAttackRange()
    {
        playerInAttackRange = true;
        movementController.SendMessage("OnTargetInRange");
    }

    void OutOfAttackRange()
    {
        playerInAttackRange = false;
        movementController.SendMessage("OnTargetOutsideRange");
    }

    void AllyBlockingSight()
    {
        movementController.SendMessage("SpreadOut");
    }
}
