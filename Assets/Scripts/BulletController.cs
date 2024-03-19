using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] float speed = 5;

    void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;

        transform.Translate(movement);

    }
}
