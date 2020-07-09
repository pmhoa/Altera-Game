﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour, IUnit, IHit
{

    //public CapsuleCollider rangeColl;
    [SerializeField] private float range = 0;
    public float Range { get => range; }
    public NavMeshAgent Agent;
    public bool moving { get; set; }
    private LineRenderer line;
    private TileScript currentTile = null;
    [SerializeField] private WeaponStats weapon;
    public WeaponStats Weapon { get => weapon; }

    [SerializeField] private UnitStats stats;
    public UnitStats Stats { get => stats; }

    //private bool playerTurn;
    private MainControl mc;
    private CameraControl cam;
    public Transform rotator;

    [SerializeField] private GameObject bulletPf;
    public Transform bulletPoint;


    public bool canShoot;
    public class MoveSet
    {
        public bool move;
        public bool action;
    }
    private MoveSet moves = new MoveSet();
    public MoveSet Moves { get => moves; set => moves = value; }

    private void Awake()
    {
        Moves.move = true;
        Moves.action = true;
        Agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        mc = MainControl.Instance;
        cam = mc.camControl;
    }
    void Start()
    {
        //rangeColl.radius = range;
        //StartCoroutine(MoveCheck());
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Fire();
        }
    }
    public bool control()
    {
        //playerTurn = true;
        return true;
    }
    public void TakeHit(Hit hit)
    {
        Debug.Log(hit.Dmg);
    }
    public void StartTurn()
    {
        //playerTurn = true;
        mc.ChangePlayer(this);
        mc.playerTurn = true;
        mc.TileCheck();
    }
    public void Death()
    {
        gameObject.SetActive(false);
        mc.UpdateUnits();
    }
    public void MoveUnit(TileScript tile)
    {

        if (currentTile)
        {
            TileScript lasttile = currentTile;
            lasttile.Taken = false;
        }

        currentTile = tile;
        currentTile.Taken = true;
        MovePlayer(tile.transform.position);

    }
    public void MovePlayer(Vector3 pos)
    {
        if (!moving && Agent.enabled == true && Moves.move == true)
        {
            moving = true;
            ResetPath();
            Agent.SetDestination(pos);
            StartCoroutine(MoveCheck());
        }
    }
    public IEnumerator MoveCheck()
    {
        yield return new WaitForEndOfFrame();
        while (Agent.remainingDistance != 0)
            yield return null;
        moving = false;
        Moves.move = false;
        mc.TileCheck();
        UserInterface.Instance.turn.interactable = true;
        //EndTurn();
    }
    public void Fire()
    {
        if (Moves.action == true)
            StartCoroutine(FireMain(cam.hitChange));
    }
    public IEnumerator FireMain(float hitChange)
    {
        GameObject nbullet = Instantiate(bulletPf, bulletPoint);
        Bullets nb = nbullet.GetComponent<Bullets>();
        nb.speed = weapon.Speed;
        nb.hit.Dmg = weapon.BaseDmg;
        float hitRandom = Random.value;
        if (hitChange < hitRandom)
            nb.transform.eulerAngles = new Vector3(90, 23, 52);
        nb.Fire();
        nbullet.transform.SetParent(null);
        canShoot = false;
        cam.LockCam();
        Moves.action = false;
        yield return new WaitForSeconds(1.5f);
        cam.ChangeCam();
        cam.LockCam();
    }
    public void EndTurn()
    {
        if (!moving)
        {
            mc.NextTurn();
            ResetMoves();
        }
    }
    public void ResetMoves()
    {
        Moves.move = true;
        Moves.action = true;
        canShoot = false;
    }
    public void PathLine(NavMeshPath path)
    {
        if (!moving)
            StartCoroutine(DrawPath(path));
    }
    public IEnumerator DrawPath(NavMeshPath path)
    {
        if (line.positionCount != 0)
        {
            line.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z));

            if (path.corners.Length < 2)
                yield return null;

            line.positionCount = path.corners.Length;
            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 cornerPos = new Vector3(path.corners[i].x, path.corners[i].y + 0.3f, path.corners[i].z);
                line.SetPosition(i, cornerPos);
            }

            yield return null;
        }
        else
        {
            yield return null;
        }
    }
    private void ResetPath()
    {
        Agent.ResetPath();
        line.positionCount = 1;
    }
    public float HitChange(float aim, float acc, float range, float dodge)
    {
        float change = ((aim + acc) / 2 - (dodge + range) / 2 + 5) / 10;
        if (change <= 0)
            change = 0.03f;
        return change;
    }
    public float HitRange(WeaponStats weapon, Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        float[] ranges = weapon.Ranges;
        //Debug.Log($"{distance}");
        if (distance >= ranges[4])
        {
            //Debug.Log(4);
            return 16;
        }
        else if (distance >= ranges[3])
        {
            //Debug.Log(3);
            return 8;
        }
        else if (distance >= ranges[2])
        {
            //Debug.Log(2);
            return 4;
        }
        else if (distance >= ranges[1])
        {
            //Debug.Log(1);
            return 1;
        }
        else
        {
            //Debug.Log(0);
            return -2;
        }
    }
}
