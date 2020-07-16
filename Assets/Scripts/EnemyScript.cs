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
    [SerializeField] private Transform rayPos;
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
        StopCoroutine("FindPlayer");
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
        StartCoroutine("FindPlayer");
        while (!mc.combat)
        {
            yield return new WaitForSeconds(Random.Range(2f, 6f));
            MoveUnit(Mc.ChooseRandomTile(FindUnitTiles()));
        }
    }
    public IEnumerator FindPlayer()
    {
        while (!mc.combat)
        {
            yield return new WaitForSeconds(0.25f);
            if (SeePlayer() != null)
                mc.StartCombat();
        }

    }
    public GameObject JoinCombat(Transform combatOrigin)
    {
        if (Vector3.Distance(transform.position, combatOrigin.position) < stats.MoveRange * 2)
        {
            return gameObject;
        }
        else 
            return null;
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
    public PlayerControl SeePlayer()
    {
        PlayerControl[] playerInScene = FindObjectsOfType<PlayerControl>();
        List<PlayerControl> viablePc = new List<PlayerControl>();
        foreach (PlayerControl pc in playerInScene)
        {
            Vector3 pAngle = pc.gameObject.transform.position - transform.position;
            Vector3 eAngle = transform.forward * 8f;

            float angle = Vector3.Angle(pAngle, eAngle);
            if (angle <= 65f)
            {
                //Debug.Log(pc.gameObject.name);
                float distance = Vector3.Distance(Mc.xzVector(transform.position), Mc.xzVector(pc.gameObject.transform.position));
                //Debug.Log(distance);
                if (distance < 8f)
                    viablePc.Add(pc);
            }
            //Debug.Log(gameObject.name + " " + angle);
        }
        if (viablePc.Count > 0)
        {
            PlayerControl currentPc = null;
            float currentDistance = 200;
            foreach (PlayerControl pc in viablePc)
            {
                RaycastHit hit;
                if (Physics.Linecast(rayPos.position, pc.gameObject.transform.position, out hit))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.distance <= currentDistance && hit.collider.gameObject.GetComponent<PlayerControl>())
                    {
                        currentDistance = hit.distance;
                        currentPc = pc;
                    }
                }
                Debug.Log(currentDistance);
            }
            if (currentPc)
                return currentPc;
            else
                return null;
        }
        else
            return null;
    }
}
