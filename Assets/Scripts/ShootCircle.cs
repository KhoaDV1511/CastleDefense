using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCircle : MonoBehaviour
{
    private float _speed;
    private float _maxSpeed;

    private Coroutine _speedUp;
    // Start is called before the first frame update
    void Awake()
    {
        _maxSpeed = 9;
        _speed = 0.01f;
        Signals.Get<OnStopGame>().AddListener(StopAttack);
        Signals.Get<EnemyPosProjectile>().AddListener(TrackMovement);
        if(_speedUp != null) StopCoroutine(_speedUp);
        _speedUp = StartCoroutine(SpeedUp());
    }

    private void OnDestroy()
    {
        Signals.Get<OnStopGame>().RemoveListener(StopAttack);
        Signals.Get<EnemyPosProjectile>().RemoveListener(TrackMovement);
    }
    private void StopAttack()
    {
        Destroy(gameObject);
    }

    private void TrackMovement(Vector3 enemy)
    {
        transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        //transform.up = enemy - transform.position;
        //if(transform.position == enemy) Destroy(gameObject);
    }
    IEnumerator SpeedUp()
    {
        for (int i = 1; i <= _maxSpeed; i++)
        {
            yield return new WaitForSeconds(0.3f/_maxSpeed);
            _speed++;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
