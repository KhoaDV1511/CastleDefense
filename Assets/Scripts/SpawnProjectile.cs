using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    [SerializeField] private Transform objShoot;
    [SerializeField] private GameObject skill;
    private Collider2D[] _enemysInsideArea;

    [SerializeField] private float radius;
    [SerializeField] LayerMask mask;
    private Coroutine _sweep;

    private float _sweepFrequency = 0.01f;
    private float _quantityHealthCastle = 100;

    private List<Vector3> _enemyPos = new List<Vector3>();
    private List<float> _distanceEnemy = new List<float>();

    private Vector3 _posSkill;
    
    private const int MANA_COMBATANT = 10;
    private const string THUNDER = "Thunder";
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int TIME_COOLDOWN_SKILL = 10;
    private Tween _coolDownSkill;

    private bool _isCoolDown = false;
    private bool _startInvoke = false;
    private float _startTime;
    private float _timeCoolDownRemain;
    // Start is called before the first frame update
    void Awake()
    {
        _timeCoolDownRemain = 0;
        _startTime = 0;
        _startInvoke = false;
        _isCoolDown = false;
        Signals.Get<ThunderSkills>().AddListener(Thunder);
        Signals.Get<OnStopGame>().AddListener(StopSpawn);
        Signals.Get<StartFindEnemy>().AddListener(StartSpawn);
        Signals.Get<TimeReduce>().AddListener(SkillAfterReduce);
    }
    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && _enemysInsideArea.Length > 0 && !_isCoolDown)
            Signals.Get<ManaUse>().Dispatch(THUNDER, MANA_COMBATANT);
    }
    private void SkillAfterReduce(float reduce)
    {
        _coolDownSkill?.Kill();
        _timeCoolDownRemain = TIME_COOLDOWN_SKILL - (Time.time - _startTime) + reduce;
        _coolDownSkill = DOVirtual.DelayedCall(_timeCoolDownRemain, () => _isCoolDown = false);
    }

    private void Thunder()
    {
        _isCoolDown = true;
        _startTime = Time.time;
        Signals.Get<CoolDownBarThunder>().Dispatch(TIME_COOLDOWN_SKILL);
        _coolDownSkill = DOVirtual.DelayedCall(TIME_COOLDOWN_SKILL, () =>
        {
            _isCoolDown = false;
        });
        skill.transform.position = _posSkill;
        skill.SetActive(true);
    }
    private void StartSpawn()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
        _startInvoke = false;
    }

    private void StopSpawn()
    {
        _coolDownSkill?.Kill();
        _isCoolDown = false;
        if(_sweep != null) StopCoroutine(_sweep);
        CancelInvoke();
    }
    private void SpawnProjectiles()
    {
        var obj = Instantiate(objShoot, objShoot.transform.position, Quaternion.identity).gameObject;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    private IEnumerator EnemyPos()
    {
        _enemyPos.Clear();
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        //if(_quantityHealthCastle <= 0) yield break;
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
        foreach (var dis in _enemyPos)
        {
            float dist = Vector3.Distance(dis, transform.position);
            _distanceEnemy.Add(dist);
        }
        var min_dis = _distanceEnemy.AsQueryable().Min();
        var indexInList = _distanceEnemy.IndexOf(min_dis);
        if(!_startInvoke)
        {
            InvokeRepeating(nameof(SpawnProjectiles), 0, 1f);
            _startInvoke = true;
        }
        Signals.Get<EnemyPosProjectile>().Dispatch(_enemyPos[indexInList]);
        _posSkill = _enemyPos[indexInList];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
