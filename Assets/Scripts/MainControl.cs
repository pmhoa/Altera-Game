using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class MainControl : MonoBehaviour
{
    private static MainControl _instance { get; set; }
    public static MainControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainControl>();
            }

            return _instance;
        }
    }

    public delegate void MainEvents();
    public MainEvents CheckTiles;

    private UserInterface ui;
    public CameraControl camControl;
    public List<GameObject> units = new List<GameObject>();
    private int turnOrder = 0;
    public PlayerControl currentPc;
    public GameObject currentUnitObj;
    public IUnit currentUnit;
    public bool playerTurn;
    private List<TileScript> tiles = new List<TileScript>();


    // Start is called before the first frame update
    private void Awake()
    {
        ui = UserInterface.Instance;
        foreach (TileScript ts in FindObjectsOfType<TileScript>())
        {
            tiles.Add(ts);
        }
        /*
        foreach (EnemyScript unit in FindObjectsOfType<EnemyScript>())
        {
            units.Add(unit.transform.gameObject);
        }
        */
        foreach (PlayerControl unit in FindObjectsOfType<PlayerControl>())
        {
            currentPc = unit;
            camControl.pc = unit;
        }
        UpdateUnits();

    }
    void Start()
    {
        NextTurn();
    }
    public void UpdateUnits()
    {
        units.Clear();
        foreach (GameObject unit in FindObjectsOfType<GameObject>())
        {
            if (unit.GetComponent<IUnit>() != null && unit.activeSelf)
            {
                units.Add(unit);
            }
        }
        if (turnOrder >= units.Count)
            turnOrder = 0;
    }
    public void TileCheck()
    {
        if (CheckTiles != null) CheckTiles.Invoke();
    }
    public void NextTurn()
    {
        playerTurn = false;
        CheckTiles();
        ui.turn.interactable = false;
        GameObject cu = units[turnOrder];
        currentUnitObj = cu;
        currentUnit = units[turnOrder].GetComponent<IUnit>();
        currentUnit.StartTurn();
        camControl.follow = cu.transform;
        if (turnOrder + 1 >= units.Count)
            turnOrder = 0;
        else
            turnOrder++;
    }
    public void ChangePlayer(PlayerControl pc)
    {
        currentPc = pc;
        camControl.pc = pc;
    }
    public void EndPlayerTurn()
    {
        currentPc.EndTurn();
    }

    public static float WalkDistance(Vector3 to, NavMeshAgent usedAgent)
    {
        NavMeshPath path = new NavMeshPath();
        usedAgent.CalculatePath(to, path);
        return CalcPathDistance(path);
    }
    public static float CalcPathDistance(NavMeshPath path)
    {
        float sum = 0;
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 xzpos1 = new Vector3(path.corners[i].x, 0, path.corners[i].z);
            Vector3 xzpos2 = new Vector3(path.corners[i - 1].x, 0, path.corners[i - 1].z);
            sum += Vector3.Distance(xzpos2, xzpos1);
        }
        return sum;
    }
    public static TileScript ChooseTile(List<TileScript> tiles, NavMeshAgent targetAgent)
    {
        TileScript currentTile = null;
        float currentDistance = float.MaxValue;
        foreach (TileScript ts in tiles)
        {
            float distance = WalkDistance(ts.transform.position, targetAgent);
            if (distance <= currentDistance)
            {
                currentTile = ts;
                currentDistance = distance;
            }
        }
        //Debug.Log(currentDistance);
        return currentTile;
    }
    public static List<TileScript> FindTiles(NavMeshAgent agent, float moveRange, Transform transformPos)
    {
        List<TileScript> tiles = new List<TileScript>();
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (tile.CheckRange(moveRange, agent, transformPos) && !tile.Taken)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
}
