using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Color og;
    private Color rangeColor;
    private Color activeColor;
    private Material mat;
    private bool inRange;
    private int state;
    private PlayerControl pc;
    private NavMeshPath path;
    public GameObject centerPoint;
    public GameObject iconObj;
    public int tileType;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        og = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a);
        rangeColor = new Color(mat.color.r, mat.color.g, mat.color.b, 0.25f);
        activeColor = new Color(mat.color.r, mat.color.g, mat.color.b, 0.8f);
    }
    private void Awake()
    {
        pc = PlayerControl.Instance;
        pc.CheckTiles += CheckState;
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //CheckState();
    }
    private void OnMouseDown()
    {
        if (inRange)
        {
            pc.MovePlayer(transform.position);
            pc.ctile = this;
        }
    }
    private void OnMouseEnter()
    {
        if (inRange)
            ChangeState(2);
    }
    private void OnMouseExit()
    {
        if (inRange)
            ChangeState(1);
        else
            ChangeState(0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            ChangeState(-1);
        }
        if (other.tag == "Cover")
        {
            tileType = 1;
            iconObj.SetActive(true);
        }
        if (other.tag == "Movement")
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
    public void CheckState()
    {
        inRange = CheckRange();
        if (state != -1)
        {
            if (pc.moving)
            {
                ChangeState(0);
            }
            else
            {
                if (inRange)
                    ChangeState(1);
                else
                    ChangeState(0);
            }

        }

    }
    public void ChangeState(int s)
    {
        state = s;
        if (state == -1)
        {
            gameObject.SetActive(false);
            pc.CheckTiles -= CheckState;
            centerPoint.SetActive(false);
        }
        else if (state == 0)
        {
            mat.color = og;
            inRange = false;
            centerPoint.SetActive(false);
        }
        else if (state == 1)
        {
            mat.color = rangeColor;
            inRange = true;
            centerPoint.SetActive(false);
        }
        else if (state == 2)
        {
            mat.color = activeColor;
            centerPoint.SetActive(true);
            if (path.corners.Length > 0)
                pc.PathLine(path);
        }
    }
    public float CalcPathDistance(NavMeshPath path)
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

    public bool CheckRange()
    {
        float r = pc.range;
        float maxDistance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(pc.transform.position.x, 0, pc.transform.position.z));

        if (maxDistance <= r)
        {
            path = new NavMeshPath();
            pc.agent.CalculatePath(transform.position, path);
            float distance = CalcPathDistance(path);
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
}
