using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public UserInterface ui;
    public CameraControl camControl;
    public List<UnitStats> units = new List<UnitStats>();
    public int turnOrder = 0;
    public PlayerControl currentPc;
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
        foreach (EnemyScript unit in FindObjectsOfType<EnemyScript>())
        {
            units.Add(unit.stats);
        }
        foreach (PlayerControl unit in FindObjectsOfType<PlayerControl>())
        {
            units.Add(unit.stats);
            currentPc = unit;
            camControl.pc = unit;
        }
    }
    void Start()
    {
        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TileCheck()
    {
        if (CheckTiles != null) CheckTiles.Invoke();
    }
    public void NextTurn()
    {
        ui.turn.interactable = false;
        UnitStats cu = units[turnOrder];
        if (units[turnOrder].pcontrol)
        {
            playerTurn = true;
            PlayerControl control = cu.obj.GetComponent<PlayerControl>();
            currentPc = control;
            camControl.follow = control.gameObject.transform;
            camControl.pc = control;
            control.playerTurn = true;
            TileCheck();
        }
        else
        {
            EnemyScript control = cu.obj.GetComponent<EnemyScript>();
            camControl.follow = control.gameObject.transform;
            control.StartTurn();
        }
        if (turnOrder + 1 >= units.Count)
            turnOrder = 0;
        else
            turnOrder++;
    }
    public void EndPlayerTurn()
    {
        currentPc.EndTurn();
    }

}
