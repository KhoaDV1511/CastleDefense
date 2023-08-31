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
    // Start is called before the first frame update
    private void Awake()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _speed = 2;
        _posRandom = Random.Range(0, posAIMove.Count - 1);
        Signals.Get<CastlePos>().AddListener(TrackMovement);
        Signals.Get<PosAIMove>().AddListener(PosAIMoveTo);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(CastlePos());
    }

    private void OnDestroy()
    {
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
        _agent.SetDestination(castle);
        //transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        //transform.up = enemy - transform.position;
    }
    private IEnumerator CastlePos()
    {
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        if(_enemysInsideArea.Length > 0)
        {
            _agent.enabled = false;
            _obstacle.enabled = true;
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
