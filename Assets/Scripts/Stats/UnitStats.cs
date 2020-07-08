using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{

    private float _health;
    private float _aim;
    private float _dodge;

    public float Health
    {
        get => _health;
        set => _health = value;
    }
    public float Aim
    {
        get => _aim;
        set => _aim = value;
    }
    public float Dodge
    {
        get => _dodge;
        set => _dodge = value;
    }

  

}
