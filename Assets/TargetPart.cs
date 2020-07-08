using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour, IHit
{
    public EnemyScript parent;
    public string partName;
    public float hitMultiplier = 1;

    private void Start()
    {
        parent = GetComponentInParent<EnemyScript>();
    }
    public void TakeHit(Hit hit)
    {
        Debug.Log(hit.Dmg);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Coll");
    }
    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Trigger Coll");
    }
}
