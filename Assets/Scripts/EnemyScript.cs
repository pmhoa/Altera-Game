using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    public UnitStats stats;
    public float moveRange;
    public List<TileScript> tilesInRange;
    public TileScript currentTile;
    public NavMeshAgent agent;
    public MainControl mc;


    private void Start()
    {
        mc = MainControl.Instance;
      //  stats.obj = gameObject;

    }
    public void FindTiles()
    {
        tilesInRange.Clear();
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (tile.CheckRange(moveRange, agent, transform) && !tile.taken)
            {

                tilesInRange.Add(tile);
            }
        }
    }
    public void StartTurn()
    {
        StopCoroutine("MoveRotation");
        StartCoroutine("MoveRotation");
    }
    public IEnumerator MoveRotation()
    {
        yield return new WaitForSeconds(1f);
        agent.ResetPath();
        FindTiles();
        if (currentTile)
        {
            currentTile.taken = false;
            if (currentTile.inRange)
                currentTile.ChangeState(1);
            else
                currentTile.ChangeState(0);
        }
        currentTile = tilesInRange[Random.Range(0, tilesInRange.Count)];
        currentTile.taken = true;
        currentTile.ChangeColor(3);
        Transform movePoint = currentTile.transform;
        agent.SetDestination(movePoint.position);
        yield return new WaitForSeconds(0.5f);
        while (agent.remainingDistance != 0)
            yield return null;
        mc.NextTurn();
    }
}
