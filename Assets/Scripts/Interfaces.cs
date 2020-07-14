﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void StartTurn();
    void MoveUnit(TileScript tile);
    void Death();
    void ResetMoves();
    void MoveToClosestTile();
    MoveSet Moves {get; set;}
    UnitStats Stats { get; set; }
    WeaponStats Weapon { get; set; }

}
public class MoveSet
{
    public bool move;
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
}

[System.Serializable]
public class Target
{
    public string targetName;
    public float hitMod;
}
