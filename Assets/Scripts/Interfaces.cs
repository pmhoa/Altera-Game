using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IUnit
{
    void StartTurn();
    void MoveUnit(TileScript tile);
    void Death();
    void ResetMoves();
    void MoveToClosestTile();
    GameObject JoinCombat(Transform combatOrigin);
    MoveSet Moves {get; set;}
    UnitStats Stats { get; set; }
    WeaponStats Weapon { get; set; }
    NavMeshAgent Agent { get; set; }
    Transform UnitTransform();
}
[System.Serializable]
public class MoveSet
{
    public bool moving;
    public bool move;

    public bool aiming;
    public bool action;
}
public interface IHit
{
    void TakeHit(Hit hit);
}

public class Hit
{
    public float Dmg { get; set; }
}

public interface ITargetable
{
    UnitStats targetStats();
    Target Target { get; set; }
    float HitChange(float aim, float acc, float range, ITargetable target);
    float TileMod();
    Transform TargetTransform();
}

[System.Serializable]
public class Target
{
    public string targetName;
    public float hitMod;
}
