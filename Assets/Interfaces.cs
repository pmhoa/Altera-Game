using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void StartTurn();
    void MoveUnit(TileScript tile);
}
public interface IHit
{
    void TakeHit(Hit hit);
}
public class Hit
{
    public float Dmg { get; set; }
}