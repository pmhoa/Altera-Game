using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour,IDamagable
{
    public UnitStats unitStats;

    public void TakeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        unitStats = new UnitStats();
        unitStats.Health = 100;
    }

}
