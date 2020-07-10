using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    }
    public void TileCheck()
    {
        if (CheckTiles != null) CheckTiles.Invoke();
    }
    public void NextTurn()
    {
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

}
