using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour, IUnit
{
    // Start is called before the first frame update
    public UnitStats stats;
    [SerializeField] private float moveRange;
    private List<TileScript> tilesInRange = new List<TileScript>();
    private TileScript currentTile;
    private NavMeshAgent agent;
    private MainControl mc;
    

    private void Start()
    {
        mc = MainControl.Instance;
        stats.Hp = stats.Hpmax;
        agent = GetComponent<NavMeshAgent>();
    }
    public void FindTiles()
    {
        tilesInRange.Clear();
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (tile.CheckRange(moveRange, agent, transform) && !tile.Taken)
            {
                tilesInRange.Add(tile);
            }
        }
    }
    public bool control()
    {
        return false;
    }
    public void StartTurn()
    {
        StopCoroutine("MoveRotation");
        StartCoroutine("MoveRotation");
    }
    public void Death()
    {
        gameObject.SetActive(false);
        mc.UpdateUnits();
    }

    public void MoveUnit(TileScript tile)
    {
        agent.ResetPath();
        if (currentTile)
        {
            currentTile.Taken = false;
            currentTile.ChangeState(0);
        }
        currentTile = tile;
        agent.SetDestination(tile.transform.position);
        tile.Taken = true;
        tile.ChangeColor(3);
    }
    public IEnumerator MoveRotation()
    {
        yield return new WaitForSeconds(1f);
        FindTiles();
        MoveUnit(tilesInRange[Random.Range(0, tilesInRange.Count)]);
        yield return new WaitForSeconds(0.5f);
        while (agent.remainingDistance != 0)
            yield return null;
        mc.NextTurn();
    }
}
