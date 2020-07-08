using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    public delegate void PlayerEvents();
    public PlayerEvents CheckTiles;
    public NavMeshAgent agent;
    public float range;
    //public CapsuleCollider rangeColl;
    public bool moving;
    private LineRenderer line;
    public TileScript currentTile;
    public WeaponClass weapon;
    public UnitStats stats;

    public bool playerTurn;
    public MainControl mc;
    public class MoveSet
    {

    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        //rangeColl.radius = range;
        TileCheck();
      //  stats.pcontrol = true;
      //  stats.obj = gameObject;
        mc = MainControl.Instance;
        //StartCoroutine(MoveCheck());
    }
    public void MovePlayer(Vector3 pos)
    {
        if (!moving && agent.enabled == true)
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
        moving = false;
        mc.TileCheck();
        mc.ui.turn.interactable = true;

        //EndTurn();
    }
    public void EndTurn()
    {
        if (!moving)
        {
            mc.playerTurn = false;
            mc.NextTurn();
        }

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
