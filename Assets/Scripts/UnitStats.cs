
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    [SerializeField] private float hpmax;
    public float Hp { get; set; }

    [SerializeField] private float aim;
    [SerializeField] private float dodge;

    public float Hpmax { get => hpmax; set => hpmax = value; }
    public float Aim { get => aim; set => aim = value; }
    public float Dodge { get => dodge; set => dodge = value; }
}