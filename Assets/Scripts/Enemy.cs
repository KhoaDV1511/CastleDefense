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
    private float _speed;
    private Coroutine _sweep;
    
    private float _sweepFrequency = 0.01f;

    private List<Vector3> posAIMove = new List<Vector3>();

    private int _posRandom;

    private Coroutine _aIMoveAround;

    private Vector3 _castlePos;
    // Start is called before the first frame update
    private void Awake()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _speed = 2;
        Signals.Get<CastlePos>().AddListener(TrackMovement);
        Signals.Get<PosAIMove>().AddListener(PosAIMoveTo);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }

    private void OnDestroy()
    {
        //Signals.Get<CastlePos>().Dispatch(_castlePos);
        Signals.Get<CastlePos>().RemoveListener(TrackMovement);
        Signals.Get<PosAIMove>().RemoveListener(PosAIMoveTo);
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
        if(posAIMove.Count == 0)
            posAIMove.Add(pos);
    }
    private void TrackMovement(Vector3 castle)
    {
        if(!_agent.enabled) return;
        _castlePos = castle;
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(castle, path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            _posRandom = (int)Random.Range(0, posAIMove.Count - 1);
            _agent.avoidancePriority = Random.Range(5, 24);
            _agent.SetDestination(posAIMove[_posRandom]);
        }
        else
        {
            _agent.avoidancePriority = Random.Range(25, 45);
            if(_aIMoveAround != null) StopCoroutine(AIMoveAround(castle));
            _agent.SetDestination(castle);
        }
        
        //transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        //transform.up = enemy - transform.position;
    }

    private IEnumerator AIMoveAround(Vector3 castlePos)
    {
        yield return new WaitForSeconds(1);
        
        yield return new WaitForSeconds(2);
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
            TrackMovement(_castlePos);
            yield break;
        }
 
        yield return new WaitForSeconds(_sweepFrequency);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
