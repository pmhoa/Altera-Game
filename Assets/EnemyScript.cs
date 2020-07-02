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
    public PlayerControl pc;


    private void Start()
    {
        pc = PlayerControl.Instance;
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
        if (!pc.playerTurn)
        {
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
            yield return new WaitForEndOfFrame();
            while (agent.remainingDistance != 0)
                yield return null;
            pc.playerTurn = true;
            pc.TileCheck();
        }
    }
}
