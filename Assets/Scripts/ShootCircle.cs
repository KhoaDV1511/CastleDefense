using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCircle : MonoBehaviour
{
    private float _speed;

    private Coroutine _speedUp;
    // Start is called before the first frame update
    void Awake()
    {
        _speed = 0.01f;
        Signals.Get<EnemyPosProjectile>().AddListener(TrackMovement);
        if(_speedUp != null) StopCoroutine(_speedUp);
        _speedUp = StartCoroutine(SpeedUp());
    }

    private void OnDestroy()
    {
        Signals.Get<EnemyPosProjectile>().RemoveListener(TrackMovement);
    }

    private void TrackMovement(Vector3 enemy)
    {
        transform.position = Vector2.MoveTowards(transform.position, enemy, _speed * Time.deltaTime);
        //transform.up = enemy - transform.position;
        if(transform.position == enemy) Destroy(gameObject);
    }
    IEnumerator SpeedUp()
    {
        for (int i = 1; i <= 7; i++)
        {
            yield return new WaitForSeconds(0.3f/7);
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
