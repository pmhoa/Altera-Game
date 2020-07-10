using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour, IHit, ITargetable
{
    public EnemyScript parent;
    [SerializeField] private Target _target;
    public Target Target { get => _target; set => _target = value; }

    public UnitStats stats() { return parent.Stats; }
    private void Start()
    {
        parent = GetComponentInParent<EnemyScript>();
    }
    public void TakeHit(Hit hit)
    {
        //Debug.Log(hit.Dmg);
        parent.Stats.Hp -= hit.Dmg;
        if (parent.Stats.Hp <= 0)
            parent.GetComponent<IUnit>().Death();
    }
    public float HitChange(float aim, float acc, float range, float dodge)
    {
        float change = ((aim + acc) / 2 - (dodge + range) / 2 + 5) / 10;
        if (change <= 0)
            change = 0.03f;
        return change;
    }
    
}
