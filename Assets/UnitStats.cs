
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    [SerializeField] private float hp;
    [SerializeField] private float aim;
    [SerializeField] private float dodge;

    public float Hp { get => hp; set => hp = value; }
    public float Aim { get => aim; set => aim = value; }
    public float Dodge { get => dodge; set => dodge = value; }
}