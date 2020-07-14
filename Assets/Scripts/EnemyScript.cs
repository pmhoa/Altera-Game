using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mc = MainControl; //used to make calling static methods shorter
public class EnemyScript : MonoBehaviour, IUnit
{
    // Start is called before the first frame update
    [SerializeField] private UnitStats stats;
    [SerializeField] private WeaponStats weapon;
    [SerializeField] private float moveRange;
    [SerializeField]private List<TileScript> tilesInRange = new List<TileScript>();
    private TileScript currentTile;
    private NavMeshAgent agent;
    private MainControl mc;
    private MoveSet moves = new MoveSet();
    public MoveSet Moves { get => moves; set => moves = value; }
    public UnitStats Stats { get => stats; set => stats = value; }
    public WeaponStats Weapon { get => weapon; set => weapon = value; }
    public TileScript CurrentTile { get => currentTile; }

    private void Start()
    {
        mc = MainControl.Instance;
        stats.Hp = stats.Hpmax;
        agent = GetComponent<NavMeshAgent>();
        ResetMoves();
        MoveToClosestTile();
        //tilesInRange = FindTiles();
    }
    public List<TileScript> FindTiles()
    {
        List<TileScript> tiles = new List<TileScript>();
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (tile.CheckRange(moveRange, agent, transform) && !tile.Taken)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
    private TileScript ChooseTile(List<TileScript> tiles, NavMeshAgent targetAgent)
    {
        TileScript current = null;
        float currentDistance = float.MaxValue;
        foreach (TileScript ts in tiles)
        {
            float distance = WalkDistance(ts.transform.position, targetAgent);
            if (distance <= currentDistance)
            {
                current = ts;
                currentDistance = distance;
            }
        }
        Debug.Log(currentDistance);
        return current;
    }
    public void MoveToClosestTile()
    {
        MoveUnit(ChooseTile(FindTiles(), agent));
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
        if (currentTile)
            currentTile.Taken = false;
        mc.UpdateUnits();
    }
    public void ResetMoves()
    {
        Moves.move = true;
        Moves.action = true;
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
        Moves.move = false;
    }
    public IEnumerator MoveRotation()
    {
        ResetMoves();
        yield return new WaitForSeconds(1f);
        PlayerControl apc = NearestPlayer();
        if (apc)
            while (moves.action || moves.move)
            {
                if (TestAttack(apc.transform.position) && moves.action)
                {
                    Attack(apc.GetComponent<IHit>(), apc.GetComponent<ITargetable>());
                }
                else if (moves.move)
                {
                    MoveUnit(ChooseTile(FindTiles(), apc.Agent));
                    yield return new WaitForSeconds(0.5f);
                    while (agent.remainingDistance != 0)
                        yield return null;
                }
                yield return new WaitForSeconds(1f);
            }
        //Debug.Log(TestAttack(apc.transform.position));

        yield return new WaitForSeconds(1.25f);
        ResetMoves();
        mc.NextTurn();
    }
    private float WalkDistance(Vector3 to, NavMeshAgent usedAgent)
    {
        NavMeshPath path = new NavMeshPath();
        usedAgent.CalculatePath(to, path);
        return CalcPathDistance(path);
    }
    private float CalcPathDistance(NavMeshPath path)
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
    private bool TestAttack(Vector3 targetPos)
    {
        transform.LookAt(xzVector(targetPos));
        if (weapon.Melee)
        {
            if (weapon.Ranges[0] >= Vector3.Distance(transform.position, targetPos))
                return true;
            else
                return false;
        }
        else
        {
            return true;
        }

    }
    private void Attack(IHit hitable, ITargetable target)
    {
        float random = Random.value;
        if (random > target.HitChange(stats.Aim, weapon.Accuracy, 1, target))
        {
            Hit hit = new Hit();
            hit.Dmg = weapon.BaseDmg;
            hitable.TakeHit(hit);
            moves.action = false;
        }
        else
            Debug.Log("Failed Attack");

    }

    private Vector3 xzVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
    private PlayerControl NearestPlayer()
    {
        PlayerControl currentPlayer = null;
        float currentDistance = 0;
        foreach (PlayerControl pc in FindObjectsOfType<PlayerControl>())
        {
            Transform pct = pc.transform;
            Vector3 pcXzPosition = xzVector(pct.position);
            float distance = WalkDistance(pcXzPosition, agent);
            if (currentDistance == 0)
            {
                currentPlayer = pc;
                currentDistance = distance;
            }
            else if (currentDistance > distance)
            {
                currentPlayer = pc;
                currentDistance = distance;
            }
        }
        return currentPlayer;
    }
}
