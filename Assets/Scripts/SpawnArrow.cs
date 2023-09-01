using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnArrow : MonoBehaviour
{
    [SerializeField] private Transform objShoot;
    private Collider2D[] _enemysInsideArea;

    [SerializeField] private float radius;
    [SerializeField] LayerMask mask;
    private Coroutine _sweep;

    private float _sweepFrequency = 0.005f;
    private float _quantityHealthCastle = 100;

    private List<Vector3> _enemyPos = new List<Vector3>();
    private List<float> _distanceEnemy = new List<float>();
    [HideInInspector] public Vector3 enemyPosMin;
    public static SpawnArrow Instance;
    private SpawnArrow _instance;
    private float _repeatRate = 1;

    private bool _startInvoke = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Instance = _instance;
        }
        else
        {
            Instance = _instance;
        }
        _startInvoke = false;
        Signals.Get<OnStopGame>().AddListener(StopSpawn);
        Signals.Get<StartFindEnemy>().AddListener(StartSpawn);
    }
    public void SkillArcher()
    {
        CancelInvoke();
        _repeatRate = 0.2f;
        InvokeRepeating(nameof(SpawnProjectile), 0, _repeatRate);
    }
    private void StartSpawn()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
        _startInvoke = false;
    }
    private void StopSpawn()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        CancelInvoke();
    }
    private void SpawnProjectile()
    {
        var obj = Instantiate(objShoot, objShoot.transform.position, Quaternion.identity).gameObject;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    private IEnumerator EnemyPos()
    {
        _enemyPos.Clear();
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        if(_enemysInsideArea.Length > 0)
            PosEnemyMin();
 
        yield return new WaitForSeconds(_sweepFrequency);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
    }

    private void PosEnemyMin()
    {
        foreach (Collider2D collider2D in _enemysInsideArea)
        {
            _enemyPos.Add(collider2D.transform.position);
        }

        _distanceEnemy.Clear();
        foreach (var dist in _enemyPos.Select(dis => Vector3.Distance(dis, transform.position)))
        {
            _distanceEnemy.Add(dist);
        }
        var min_dis = _distanceEnemy.AsQueryable().Min();
        var indexInList = _distanceEnemy.IndexOf(min_dis);
        if(!_startInvoke)
        {
            InvokeRepeating(nameof(SpawnProjectile), 0, _repeatRate);
            _startInvoke = true;
        }
        enemyPosMin = _enemyPos[indexInList];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
