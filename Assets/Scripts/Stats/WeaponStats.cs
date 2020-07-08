using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats 
{

    private float _accuracy;
    private float _fireRate;
    private float _damage;
    private float _range;

    public float Accuracy
    {
        get => _accuracy;
        set => _accuracy = value;
    }
    public float FireRate
    {
        get => _fireRate;
        set => _fireRate = value;
    }
    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }
    public float Range
    {
        get => _range;
        set => _fireRate = value;
    }
}
