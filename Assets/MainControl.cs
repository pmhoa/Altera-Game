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
    public GameObject currentUnit;
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
        
        foreach(GameObject unit in FindObjectsOfType<GameObject>())
        {
            if (unit.GetComponent<IUnit>() != null)
            {
                units.Add(unit);
            }
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
        GameObject cu = units[turnOrder];
        currentUnit = cu;
        units[turnOrder].GetComponent<IUnit>().StartTurn();
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
