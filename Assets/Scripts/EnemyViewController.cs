using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    [SerializeField] float viewRange;
    [SerializeField] float viewAngle; [Range(0, 180)]

    GameObject player;
    EnemyMovementController movementController;
    GameObject eyesParent;
    Renderer[] eyes;
    Color standardEyeColor;

    bool playerFound = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementController = gameObject.GetComponent<EnemyMovementController>();
        eyesParent = gameObject.transform.GetChild(1).gameObject;
        eyes = eyesParent.GetComponentsInChildren<Renderer>();
        standardEyeColor = eyes[0].material.color;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < viewRange)
        {
            if (Vector3.Angle(transform.forward, player.transform.position) < viewAngle / 2)
            {
                if (playerFound == false)
                {
                    foreach (Renderer r in eyes)
                    {
                        r.material.color = Color.red;
                    }
                    movementController.SendMessage("OnPlayerFound");
                    playerFound = true;
                }
            }
            else
            {
                if (playerFound == true)
                {
                    foreach (Renderer r in eyes)
                    {
                        r.material.color = standardEyeColor;
                    }
                    playerFound = false;
                    movementController.SendMessage("OnPlayerLost");
                }
            }
        }
        else if (playerFound == true)
        {
            playerFound = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Handles.color = Color.green;

        Vector3 viewAngleA = PosFromAngle(-viewAngle / 2);
        Vector3 viewAngleB = PosFromAngle(viewAngle / 2);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRange);
        Handles.DrawWireArc(transform.position, transform.up, viewAngleA, viewAngle, viewRange);
    }

    Vector3 PosFromAngle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
