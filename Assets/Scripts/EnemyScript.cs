using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mc = MainControl;

public class EnemyScript : MonoBehaviour, IUnit
{
    // Start is called before the first frame update
    [SerializeField] private UnitStats stats;
    [SerializeField] private WeaponStats weapon;
    [SerializeField] private float moveRange;
    private List<TileScript> tilesInRange = new List<TileScript>();
    [SerializeField] private TileScript currentTile;
    private NavMeshAgent agent;
    private MainControl mc;
    private MoveSet moves = new MoveSet();
    public MoveSet Moves { get => moves; set => moves = value; }
    public UnitStats Stats { get => stats; set => stats = value; }
    public WeaponStats Weapon { get => weapon; set => weapon = value; }
    public TileScript CurrentTile { get => currentTile; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }

    private void Start()
    {
        mc = Mc.Instance;
        stats.Hp = stats.Hpmax;
        agent = GetComponent<NavMeshAgent>();
        ResetMoves();
        StartCoroutine("OutOfCombatRoutine");
        mc.CombatStart += CombatStart;
        //StartCoroutine(ClosestWait());
    }
    public List<TileScript> FindTiles()
    {
        List<TileScript> tiles = new List<TileScript>();
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (tile.CheckRange(moveRange, agent, transform) && !tile.Taken && tile.gameObject.activeSelf)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
    public IEnumerator ClosestWait()
    {
        yield return new WaitForSeconds(0.33f);
        MoveToClosestTile();
    }
    public void MoveToClosestTile()
    {
        MoveUnit(Mc.ChooseTile(Mc.FindTiles(agent, moveRange, transform), agent));
    }
    public bool control()
    {
        return false;
    }
    public void CombatStart()
    {
        StopCoroutine("OutOfCombatRoutine");
    }
    public void StartTurn()
    {
        StopCoroutine("OutOfCombatRoutine");
        StopCoroutine("MoveRotation");
        StartCoroutine("MoveRotation");
    }
    public void Death()
    {
        gameObject.SetActive(false);
        if (currentTile)
            currentTile.Taken = false;
        currentTile.CheckState();
        mc.UpdateUnits();
    }
    public void ResetMoves()
    {
        Moves.move = true;
        Moves.action = true;
    }
    public IEnumerator OutOfCombatRoutine()
    {
        while (!mc.combat)
        {
            yield return new WaitForSeconds(Random.Range(2f, 6f));
            MoveUnit(Mc.ChooseRandomTile(FindUnitTiles()));
        }
    }
    public List<TileScript> FindUnitTiles()
    {
        return Mc.FindTiles(agent, moveRange, transform);
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
        moves.moving = true;
        StartCoroutine(MoveUnitRoutine());
    }
    public IEnumerator MoveUnitRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        while (agent.remainingDistance != 0)
            yield return null;
        moves.moving = false;
    }
    public IEnumerator MoveRotation()
    {
        ResetMoves();
        yield return new WaitForSeconds(1f);
        PlayerControl apc = NearestPlayer();
        if (apc)
            while ((moves.action && TestAttack(apc.transform.position)) || moves.move)
            {
                if (TestAttack(apc.transform.position) && moves.action && !moves.moving)
                {
                    Attack(apc.GetComponent<IHit>(), apc.GetComponent<ITargetable>());
                }
                else if (moves.move)
                {
                    MoveUnit(Mc.ChooseTile(FindTiles(), apc.Agent));
                }
                yield return new WaitForSeconds(1f);
            }
        //Debug.Log(TestAttack(apc.transform.position));

        yield return new WaitForSeconds(1.25f);
        ResetMoves();
        mc.NextTurn();
    }
    private bool TestAttack(Vector3 targetPos)
    {
        if (weapon.Melee)
        {
            if (weapon.Ranges[0] >= Vector3.Distance(transform.position, targetPos))
                return true;
            else
                return false;
        }
        else
            return true;
    }
    private void Attack(IHit hitable, ITargetable target)
    {
        transform.LookAt(Mc.xzVector(target.TargetTransform().position));
        float random = Random.value;
        if (random > target.HitChange(stats.Aim, weapon.Accuracy, 1, target))
        {
            Hit hit = new Hit();
            hit.Dmg = weapon.BaseDmg;
            hitable.TakeHit(hit);
        }
        else
            mc.SpawnText("Miss!", transform.position);
        moves.action = false;
    }

    private PlayerControl NearestPlayer()
    {
        PlayerControl currentPlayer = null;
        float currentDistance = 0;
        foreach (PlayerControl pc in FindObjectsOfType<PlayerControl>())
        {
            if (pc.transform.gameObject.activeSelf)
            {
                Transform pct = pc.transform;
                Vector3 pcXzPosition = Mc.xzVector(pct.position);
                float distance = Mc.WalkDistance(pcXzPosition, agent);
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
        }
        return currentPlayer;
    }
    public Transform UnitTransform()
    {
        return transform;
    }
}
