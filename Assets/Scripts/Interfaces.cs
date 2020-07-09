﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void StartTurn();
    void MoveUnit(TileScript tile);
    void Death();
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
    UnitStats stats();
    Target Target { get; set; }
    float HitChange(float aim, float acc, float range, float dodge);
}

[System.Serializable]
public class Target
{
    public string targetName;
    public float hitMod;
}