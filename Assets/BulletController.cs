using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float damage = 50;
    private void OnTriggerEnter(Collider other)
    {
       
        other.gameObject.GetComponent<IDamagable>().TakeDamage(100);
       
    }
}
