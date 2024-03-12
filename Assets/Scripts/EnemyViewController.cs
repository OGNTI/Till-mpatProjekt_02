using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    [SerializeField] float viewRange;
    [SerializeField] float viewAngle;

    GameObject player;
    EnemyMovementController movementController;
    GameObject eyesParent;
    Renderer[] eyes;
    Color standardEyeColor;

    bool playerInView = false;

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

        if (Vector3.Distance(transform.position, player.transform.position) < viewRange &&
            Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, dirToPlayer, out hit, viewRange);

            Debug.DrawRay(transform.position, dirToPlayer, Color.green);

            if (hit.collider != null)
            {
                viewTarget = hit.collider.gameObject;
            }
            else viewTarget = null;

            if (oldViewTarget != viewTarget && viewTarget.tag == "Player")
            {
                if (playerInView == false)
                {
                    PlayerFound();
                }
            }

        }
        else if (playerInView == true)
        {
            PlayerLost();
        }
    }

    void OnDrawGizmos()
    {

        Vector3 viewAngleA = PosFromAngle(-viewAngle / 2);
        Vector3 viewAngleB = PosFromAngle(viewAngle / 2);

        Gizmos.color = Color.blue;
        //from enemy point to player
        Gizmos.DrawLine(transform.position, transform.position + dirToPlayer.normalized);

        Gizmos.color = Color.green;
        Handles.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRange);
        Handles.DrawWireArc(transform.position, transform.up, viewAngleA, viewAngle, viewRange);
    }

    Vector3 PosFromAngle(float angle)
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
        playerInView = true;
    }

    void PlayerLost()
    {
        foreach (Renderer r in eyes)
        {
            r.material.color = standardEyeColor;
        }
        movementController.SendMessage("OnPlayerLost");
        playerInView = false;
    }
}
