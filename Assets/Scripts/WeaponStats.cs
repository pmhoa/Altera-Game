
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    [SerializeField] private float accuracy;
    [SerializeField] private float baseDmg;
    [SerializeField] private float[] ranges = new float[5];
    [SerializeField] private float speed;

    public float Accuracy { get => accuracy; set => accuracy = value; }
    public float BaseDmg { get => baseDmg; set => baseDmg = value; }
    public float[] Ranges { get => ranges; set => ranges = value; }
    public float Speed { get => speed; set => speed = value; }
}
