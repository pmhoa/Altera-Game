using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IDamagable
{

   public  UnitStats unitsts;

    public void TakeDamage(float amount)
    {
        unitsts.Health -= amount;
    }

    void Start()
    {
        unitsts = new UnitStats();
        unitsts.Health = 100;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
