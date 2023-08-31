using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshObstacle _obstacle;
    private Collider2D[] _enemysInsideArea;

    [SerializeField] private float radius;
    [SerializeField] LayerMask mask;
    private Coroutine _sweep;
    
    private float _sweepFrequency = 0.01f;

    private List<Vector3> posAIMove = new List<Vector3>();

    private int _posRandom;
    private int _health = 20;

    private Coroutine _aIMoveAround;

    private Vector3 _castlePos;

    private StopAIMoveAround _stopAIMoveAround = Signals.Get<StopAIMoveAround>();

    private bool _stopAIMove;
    // Start is called before the first frame update
    private void Awake()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _stopAIMove = false;
        Signals.Get<CastlePos>().AddListener(TrackMovement);
        Signals.Get<PosAIMove>().AddListener(PosAIMoveTo);
        _stopAIMoveAround.AddListener(StopMoveAI);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }

    private void OnDestroy()
    {
        Signals.Get<CastlePos>().RemoveListener(TrackMovement);
        Signals.Get<PosAIMove>().RemoveListener(PosAIMoveTo);
        _stopAIMoveAround.RemoveListener(StopMoveAI);
    }

    // private void Update()
    // {
    //     if (_agent.velocity.magnitude == 0)
    //     {
    //         _agent.SetDestination(posAIMove[_posRandom]);
    //     }
    // }

    private void PosAIMoveTo(Vector3 pos)
    {
        if(posAIMove.Count < 3)
            posAIMove.Add(pos);
    }
    private void TrackMovement(Vector3 castle)
    {
        if(!_agent.enabled) return;
        _castlePos = castle;
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(castle, path);
        Debug.Log("path.status: " + path.status);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            _posRandom = (int)Random.Range(0, posAIMove.Count);
            _agent.avoidancePriority = Random.Range(5, 24);
            _agent.speed = 2;
            Debug.Log("not move");
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castle));
            _aIMoveAround = StartCoroutine(AIMoveAround(castle));
        }
        else
        {
            Debug.Log("move");
            _agent.avoidancePriority = Random.Range(25, 45);
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castle));
            _agent.speed = 3.5f;
            _agent.SetDestination(castle);
        }
        
        //transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        //transform.up = enemy - transform.position;
    }

    private IEnumerator AIMoveAround(Vector3 castlePos)
    {
        yield return new WaitForSeconds(1);
        if(!_agent.enabled)
        {
            Debug.Log("AIMoveAround AIMoveAround");
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castlePos));
            yield break;
        }if(_stopAIMove)
        {
            Debug.Log("AIMoveAround AIMoveAround");
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castlePos));
            yield break;
        }
        _agent.SetDestination(posAIMove[_posRandom]);
        yield return new WaitForSeconds(1);
        if(!_agent.enabled)
        {
            Debug.Log("AIMoveAround AIMoveAround");
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castlePos));
            yield break;
        }if(_stopAIMove)
        {
            Debug.Log("AIMoveAround AIMoveAround");
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castlePos));
            yield break;
        }
        _agent.SetDestination(castlePos);
        if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castlePos));
        _aIMoveAround = StartCoroutine(AIMoveAround(castlePos));
    }
    private IEnumerator CastlePos()
    {
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        if(_enemysInsideArea.Length > 0)
        {
            _agent.enabled = false;
            _obstacle.enabled = true;
            _stopAIMoveAround.Dispatch(false);
            Signals.Get<CastlePos>().Dispatch(_castlePos);
            yield break;
        }
 
        yield return new WaitForSeconds(_sweepFrequency);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Arrow")) return;
        _health--;
        if (_health != 0) return;
        Destroy(gameObject);
        _stopAIMoveAround.Dispatch(true);
        Signals.Get<CastlePos>().Dispatch(_castlePos);
        _stopAIMoveAround.Dispatch(false);
    }

    private void StopMoveAI(bool castlePos)
    {
        _stopAIMove = castlePos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
