using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    private static PlayerControl _instance { get; set; }
    public static PlayerControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerControl>();
            }

            return _instance;
        }
    }
    public delegate void PlayerEvents();
    public PlayerEvents CheckTiles;
    public NavMeshAgent agent;
    public float range;
    public CapsuleCollider rangeColl;
    public bool moving;
    private LineRenderer line;
    public TileScript currentTile;
    public WeaponClass weapon;
    public UnitStats stats;
    public bool playerTurn;


    void Start()
    {
        playerTurn = true;
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        rangeColl.radius = range;
        TileCheck();
        //StartCoroutine(MoveCheck());
    }
    public void MovePlayer(Vector3 pos)
    {
        if (!moving && agent.enabled == true && playerTurn)
        {
            moving = true;
            TileCheck();
            ResetPath();
            agent.SetDestination(pos);
            StartCoroutine(MoveCheck());
        }
    }
    public IEnumerator MoveCheck()
    {
        yield return new WaitForEndOfFrame();
        while (agent.remainingDistance != 0)
            yield return null;
        playerTurn = false;
        moving = false;
        TileCheck();
        EndTurn();
    }
    public void EndTurn()
    {
        EnemyScript enem = FindObjectOfType<EnemyScript>();
        enem.StartTurn();
    }
    public void TileCheck()
    {
        if (CheckTiles != null) CheckTiles.Invoke();
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
        agent.ResetPath();
        line.positionCount = 1;
    }
    public float HitChange(float aim, float acc, float range, float dodge)
    {
        float change = ((aim + acc) / 2 - (dodge + range) / 2 + 5) / 10;
        if (change <= 0)
            change = 0.03f;
        return change;
    }
    public float HitRange(WeaponClass weapon, Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        float[] ranges = weapon.ranges;
        Debug.Log($"{distance}");
        if (distance >= ranges[4])
        {
            Debug.Log(4);
            return 16;
        }
        else if (distance >= ranges[3])
        {
            Debug.Log(3);
            return 8;
        }
        else if (distance >= ranges[2])
        {
            Debug.Log(2);
            return 4;

        }
        else if (distance >= ranges[1])
        {
            Debug.Log(1);
            return 1;
        }
        else
        {
            Debug.Log(0);
            return -2;
        }
    }
}
