using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour, IHit, ITargetable
{
    private EnemyScript parent;
    [SerializeField] private Target _target;
    public Target Target { get => _target; set => _target = value; }

    public UnitStats targetStats() { return parent.Stats; }
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
    public float HitChange(float aim, float acc, float range, ITargetable target)
    {
        UnitStats tStats = target.targetStats();
        float change = ((aim + acc) / 2 - (tStats.Dodge / target.TileMod() / target.Target.hitMod + range) / 2 + 5) / 10;
        if (change <= 0)
            change = 0.03f;
        return change;
    }
    public float TileMod()
    {
        if (parent.CurrentTile != null)
            return parent.CurrentTile.TileMod();
        else 
            return 1;
    }
    public Transform TargetTransform()
    {
        return parent.transform;
    }

}
