using UnityEditor;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    public float viewRange;
    public float viewAngle;
    public float attackRange;

    GameObject player;
    EnemyMovementController movementController;
    GameObject eyesParent;
    Renderer[] eyes;
    Color standardEyeColor;

    bool playerInViewRange = false;
    bool playerInAttackRange = false;

    GameObject viewTarget;

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
            RaycastHit hit;
            Physics.Raycast(transform.position+Vector3.up, dirToPlayer+Vector3.up, out hit, viewRange); //is sight blocked or not
            Debug.DrawRay(transform.position, dirToPlayer, Color.green);

            if (hit.collider != null) viewTarget = hit.collider.gameObject;
            else viewTarget = null;

            if (oldViewTarget != viewTarget && viewTarget.tag == "Player") PlayerFound();

            if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            {
                if (playerInAttackRange == false) InAttackRange();
            }
            else if (playerInAttackRange == true) OutOfAttackRange();
        }
        else if (playerInViewRange == true) PlayerLost();
    }

    public Vector3 PosFromAngle(float angle)
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
        movementController.SendMessage("OnAttack");
    }

    void OutOfAttackRange()
    {
        playerInAttackRange = false;
        movementController.SendMessage("OnAttackLost");
    }
}
