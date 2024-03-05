using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{
    [SerializeField] float viewRange = 6;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Debug.Log(transform.forward);
        if (Vector3.Distance(transform.position, player.transform.position) < viewRange)
        {
            Debug.Log("gae");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward * viewRange);
    }
}
