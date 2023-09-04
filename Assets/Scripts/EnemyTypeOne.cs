using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyTypeOne : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshObstacle obstacle;
    [SerializeField] private Transform spriteHealth;
    private Collider2D[] _enemysInsideArea;

    [SerializeField] private float radius;
    [SerializeField] LayerMask mask;
    private Coroutine _sweep;
    private Coroutine _sweepAttack;
    
    private float _sweepFrequency = 0.01f;
    private float _sweepFrequencyAtack = 1.2f;

    private List<Vector3> _posAIMove = new List<Vector3>();

    private int _posRandom;
    public int _health = 10;
    private int _healthMax;
    private int _quantity;

    private Coroutine _aIMoveAround;

    private CastlePos _castlePos = Signals.Get<CastlePos>();
    private QuantityEnemy _quantityEnemy = Signals.Get<QuantityEnemy>();
    private OnStopGame _onStopGame = Signals.Get<OnStopGame>();
    private Vector3 _castleAttack;
    private Vector3 _posAIMoveAround;

    private bool _stopAIMove;

    private const string ARROW = "Arrow";
    private const string CIRCLE_AP = "CircleAP";
    private const string CASTLE_REABLE = "CastleReable";

    private const int DAME_ERROW = 2;
    private const int DAME_CIRCLE_AP = 1;
    private const int DAME_ENEMY = 1;
    private const int DAME_BOT = 2;
    // Start is called before the first frame update
    private void Awake()
    {
        BotAttack.EnemyTypeOnes.Add(this);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        _stopAIMove = false;
        _healthMax = _health;
        _castlePos.AddListener(TrackMovement);
        _quantityEnemy.AddListener(Quantity);
        Signals.Get<PosAIMove>().AddListener(PosAIMoveTo);
        Signals.Get<StartFindEnemy>().AddListener(StartFindCastle);
        _onStopGame.AddListener(DestroyEnemy);
    }

    private void OnDestroy()
    {
        BotAttack.EnemyTypeOnes.Remove(this);
        _castlePos.RemoveListener(TrackMovement);
        _quantityEnemy.RemoveListener(Quantity);
        Signals.Get<PosAIMove>().RemoveListener(PosAIMoveTo);
        Signals.Get<StartFindEnemy>().RemoveListener(StartFindCastle);
        _onStopGame.RemoveListener(DestroyEnemy);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    private void Quantity(int quan)
    {
        _quantity = quan;
    }

    private void StartFindCastle()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
        if(_sweepAttack != null) StopCoroutine(_sweepAttack);
        _sweepAttack = StartCoroutine(AttackCastle());
    }

    private void PosAIMoveTo(Vector3 pos)
    {
        if(_posAIMove.Count < 4)
            _posAIMove.Add(pos);
    }
    private void TrackMovement(Vector3 castle)
    {
        //_posAIMoveAround = _posAIMove[2];
        if(!agent.enabled) return;
        _castleAttack = castle;
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(castle, path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            var posAIMove = _posAIMove.FindAll(s => s != _posAIMoveAround);
            _posRandom = Random.Range(0, posAIMove.Count);
            agent.SetDestination(posAIMove[_posRandom]);
            agent.avoidancePriority = Random.Range(5, 24);
        }
        else
        {
            agent.avoidancePriority = Random.Range(25, 45);
            agent.SetDestination(castle);
        }
    }
    
    private IEnumerator CastlePos()
    {
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        if(_enemysInsideArea.Length > 0)
        {
            agent.enabled = false;
            obstacle.enabled = true;
            _castlePos.Dispatch(_castleAttack);
            yield break;
        }
 
        yield return new WaitForSeconds(_sweepFrequency);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }
    
    private IEnumerator AttackCastle()
    {
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        if(_enemysInsideArea.Length > 0)
        {
            agent.enabled = false;
            obstacle.enabled = true;
            Signals.Get<DameEnemy>().Dispatch(DAME_ENEMY);
        }
 
        if(_quantity <= 0) yield break;
        yield return new WaitForSeconds(_sweepFrequencyAtack);
        if(_sweepAttack != null) StopCoroutine(_sweepAttack);
        _sweepAttack = StartCoroutine(AttackCastle());
    }

    public void DameBotAttack()
    {
        _health -= DAME_BOT;
        var scaleX = spriteHealth.transform.localScale.x - (float)DAME_BOT / _healthMax;
        if (scaleX <= 0) scaleX = 0;
        spriteHealth.transform.localScale = new Vector3(scaleX, 1, 1);
        
        if (_health > 0) return;
        _castlePos.Dispatch(_castleAttack);
        _quantity--;
        if(_quantity <= 0)
        {
            _onStopGame.Dispatch();
        }
        _quantityEnemy.Dispatch(_quantity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var spriteHealthTransform = spriteHealth.transform;
        if (other.CompareTag(CASTLE_REABLE))
        {
            _posAIMoveAround = other.gameObject.transform.position;
            TrackMovement(_castleAttack);
        }
        if (other.CompareTag(ARROW))
        {
            _health -= DAME_ERROW;
            var scaleX = spriteHealth.transform.localScale.x - (float)DAME_ERROW / _healthMax;
            if (scaleX <= 0) scaleX = 0;
            spriteHealthTransform.localScale = new Vector3(scaleX, 1, 1);
        }
        if (other.CompareTag(CIRCLE_AP))
        {
            _health -= DAME_CIRCLE_AP;
            var scaleX = spriteHealth.transform.localScale.x - (float)DAME_CIRCLE_AP / _healthMax;
            if (scaleX <= 0) scaleX = 0;
            spriteHealthTransform.localScale = new Vector3(scaleX, 1, 1);
        }
        if (_health > 0) return;
        _castlePos.Dispatch(_castleAttack);
        _quantity--;
        if(_quantity <= 0)
        {
            _onStopGame.Dispatch();
        }
        _quantityEnemy.Dispatch(_quantity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
