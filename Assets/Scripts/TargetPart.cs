using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour
{
    public EnemyScript parent;
    public string partName;
    public float hitMultiplier = 1;

    private void Start()
    {
        parent = GetComponentInParent<EnemyScript>();
    }
}
