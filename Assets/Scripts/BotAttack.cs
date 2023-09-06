using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class BotAttack : MonoBehaviour
{
    [SerializeField] private Transform barAppear;
    private NavMeshAgent _agent;

    private Tween _coolDownBar;
    private Tween _coolDownAppear;
    private Tween _speedAttack;
    private Tween _timeStopAttack;

    private int timeCoolDown;
    private float _speed;
    private float _maxSpeed;

    private Coroutine _speedUp;

    public static List<EnemyTypeOne> EnemyTypeOnes = new List<EnemyTypeOne>();

    private Coroutine _sweep;
    private Collider2D[] _enemysInsideArea;
    [SerializeField] LayerMask mask;
    private float _sweepFrequency = 0.005f;
    private List<Vector3> _enemyPos = new List<Vector3>();
    private List<float> _distanceEnemy = new List<float>();
    private bool isAttack = false;

    private OnStopGame _onStopGame = Signals.Get<OnStopGame>();

    // Start is called before the first frame update
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        isAttack = false;
        _speed = 0.01f;
        _maxSpeed = 5;
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
        if(_speedUp != null) StopCoroutine(_speedUp);
        _speedUp = StartCoroutine(SpeedUp());
        timeCoolDown = 8;
        AppearCoolDown(timeCoolDown);
        _onStopGame.AddListener(StopCoolDown);
    }

    private void Update()
    {
        if(_enemysInsideArea.Length <= 0)
        {
            transform.Translate(Vector3.right * 2 * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        _coolDownBar?.Kill();
        _coolDownAppear?.Kill();
        _speedAttack?.Kill();
        _timeStopAttack?.Kill();
        if(_sweep != null) StopCoroutine(_sweep);
        _onStopGame.RemoveListener(StopCoolDown);
    }

    IEnumerator SpeedUp()
    {
        for (int i = 1; i <= _maxSpeed; i++)
        {
            yield return new WaitForSeconds(0.3f/_maxSpeed);
            _speed++;
        }
    }

    private void StopCoolDown()
    {
        _timeStopAttack?.Kill();
        _speedAttack?.Kill();
        _coolDownBar?.Kill();
        _coolDownAppear?.Kill();
        Destroy(gameObject);
    }
    private void AppearCoolDown(int coolDown)
    {
        barAppear.localScale = new Vector3(1, 1, 0);
        _coolDownBar = barAppear.DOScaleX(0, coolDown);
        _coolDownAppear = DOVirtual.DelayedCall(coolDown,() => Destroy(gameObject));
    }
    private void TrackMovement(Vector3 enemy)
    {
        //transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        _agent.SetDestination(enemy);
        var dis = Vector3.Distance(transform.position, enemy);
        if (dis < 0.5f && !isAttack)
        {
            isAttack = true;
            // dung bot khoang t bang tg vung don danh
            _agent.speed = 0;
            _timeStopAttack = DOVirtual.DelayedCall(0.3f, () =>
            {
                _agent.speed = 3;
                // if(_speedUp != null) StopCoroutine(_speedUp);
                // _speedUp = StartCoroutine(SpeedUp());
            });
            
            DameEnemy(enemy);
            _speedAttack = DOVirtual.DelayedCall(1, () => isAttack = false);
        }
    }

    private void DameEnemy(Vector3 enemyPosMin)
    {
        var chooseEnemyAttack = EnemyTypeOnes.FindAll(e => e.transform.position == enemyPosMin);
        chooseEnemyAttack[0].DameBotAttack();
    }
    private IEnumerator EnemyPos()
    {
        _enemyPos.Clear();
        float radius = 6f;
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
        var minDis = _distanceEnemy.AsQueryable().Min();
        var indexInList = _distanceEnemy.IndexOf(minDis);
        var enemyPosMin = _enemyPos[indexInList];
        TrackMovement(enemyPosMin);
    }
}
