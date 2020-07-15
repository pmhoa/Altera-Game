using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mc = MainControl;
public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    /*
    private Color og;
    private Color rangeColor;
    private Color activeColor;
    private Color dangerColor;
    private Color coverColor;
    */
    private List<Color> ucolors = new List<Color>();
    private Material mat;

    private int state;
    private PlayerControl currentPc;
    private MainControl mc;
    private NavMeshPath currentPath;
    [SerializeField] private Renderer mainTile;
    [SerializeField] private GameObject centerPoint = null;
    [SerializeField] private GameObject iconObj = null;
    [SerializeField] private int tileType;
    public bool InRange { get; private set; }

    [SerializeField] private bool taken;
    public bool Taken { get => taken; set => taken = value; }
    public int TileType { get => tileType; set => tileType = value; }

    private Transform rotator;
    private void Awake()
    {
        mat = mainTile.material;
        currentPath = new NavMeshPath();
        Color[] cols = {
            new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a),
            new Color(mat.color.r, mat.color.g, mat.color.b, 0.35f),
            new Color(mat.color.r, mat.color.g, mat.color.b, 0.85f),
            new Color(0.85f, 0, 0, 1f),
            new Color(0, 0, 0.85f, 1f)
        };
        ucolors.AddRange(cols);
        mc = MainControl.Instance;
        //mc.CheckTiles += CheckState;
        mc.CheckUnitTiles += CheckUnitState;
    }
    private void Start()
    {
        CheckNavmesh();
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //CheckState();
    }
    private void OnMouseDown()
    {
        if ((InRange && !Taken && mc.currentUnit.Moves.move) || (!mc.combat && !Taken))
        {
            mc.currentUnit.MoveUnit(this);
            mc.currentUnit.Moves.move = false;
        }
    }
    private void OnMouseOver()
    {
        if (InRange || !mc.combat)
            ChangeState(2);
    }
    private void OnMouseExit()
    {
        if (InRange)
            ChangeState(1);
        else
            ChangeState(0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            //Debug.Log("obs");
            ChangeState(-1);
        }
        else if (other.tag == "Cover")
        {
            tileType = 1;
            iconObj.SetActive(true);
        }
        else if (other.tag == "Movement")
        {
            //CheckState();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Movement")
        {
            //ChangeState(0);
        }
    }
    public void CheckNavmesh()
    {
        NavMeshAgent agent = FindObjectOfType<PlayerControl>().Agent;
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(transform.position, path);
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            ChangeState(-1);
        }
    }
    public void CheckState()
    {
        currentPc = mc.currentPc;
        //Debug.Log("state checked");
        currentPc.Agent.enabled = true;
        InRange = CheckPcRange(currentPc);
        if (state != -1)
        {
            if (currentPc.Moves.moving || !currentPc.Moves.move)
            {
                ChangeState(0);
            }
            else
            {

                if (InRange && !Taken && mc.playerTurn)
                    ChangeState(1);
                else
                    ChangeState(0);
            }
        }
    }
    public void CheckUnitState(IUnit unit)
    {
        unit.Agent.enabled = true;
        InRange = CheckRange(unit.Stats.MoveRange, unit.Agent, unit.UnitTransform());
        if (state != -1)
        {
            if (unit.Moves.moving || !unit.Moves.move)
            {
                ChangeState(0);
            }
            else
            {

                if (InRange && !Taken && mc.playerTurn)
                    ChangeState(1);
                else
                    ChangeState(0);
            }
        }
    }
    public void LeaveTile()
    {
        Taken = false;
    }
    public void ChangeState(int s)
    {

        state = s;
        if (state == -1)
        {
            gameObject.SetActive(false);
            centerPoint.SetActive(false);
        }
        else if (state == 0)
        {
            ChangeColor(0);
            InRange = false;
            centerPoint.SetActive(false);
        }
        else if (state == 1)
        {
            ChangeColor(1);
            InRange = true;
            centerPoint.SetActive(false);
            if (tileType == 1)
                ChangeColor(4);

        }
        else if (state == 2)
        {
            if (!mc.currentUnit.Moves.aiming)
            {
                ChangeColor(2);
                centerPoint.SetActive(true);
                if (currentPath.corners.Length > 0)
                    mc.currentPc.PathLine(currentPath);
            }

        }
    }
    public void ChangeColor(int i)
    {
        mat.color = ucolors[i];
    }
    public float TileMod()
    {
        if (tileType == 1)
            return 0.7f;
        else
            return 1f;
    }

    public bool CheckRange(float range, NavMeshAgent agent, Transform unit)
    {
        float r = range;
        float maxDistance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(unit.transform.position.x, 0, unit.transform.position.z));

        if (maxDistance <= r)
        {
            currentPath = new NavMeshPath();
            agent.CalculatePath(transform.position, currentPath);
            float distance = Mc.CalcPathDistance(currentPath);
            //Debug.Log($"Distance {distance:F2} of {gameObject.name}");
            if (distance < r)
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            return false;
        }

        //Debug.Log($"{distance} of {gameObject.name}");
        //Debug.Log(distance);

    }
    public bool CheckPcRange(PlayerControl pc)
    {
        return CheckRange(pc.Stats.MoveRange, pc.Agent, pc.transform);
    }
}
