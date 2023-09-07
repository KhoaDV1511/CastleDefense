using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    private Vector3 enemyPoint;
    private Coroutine _speedUp;
    
    private float dist;
    private float nextX;
    private float baseY;
    private float height;
    
    private float targetX;
    private float archerX;
    private Vector3 targetPos;
    private float x;
    private float speed;
    private float maxSpeed;
    

    private void Awake()
    {
        maxSpeed = 12;
        speed = 0.01f;
        Signals.Get<OnStopGame>().AddListener(StopAttack);
        if(_speedUp != null) StopCoroutine(_speedUp);
        _speedUp = StartCoroutine(SpeedUp());
    }

    private void Update()
    {
        TrackMovement(Archer.Instance.enemyPosMin);
    }

    private void OnDestroy()
    {
        Signals.Get<OnStopGame>().RemoveListener(StopAttack);
    }

    private void StopAttack()
    {
        Destroy(gameObject);
    }
    private static Quaternion LookAtTarget(Vector2 rotation)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
    }
    private void TrackMovement(Vector3 enemyPoint)
    {
        archerX = firePoint.position.x;
        targetX = enemyPoint.x;

        dist = targetX - archerX;
        nextX = Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime);
        baseY = Mathf.Lerp(firePoint.position.y, enemyPoint.y, (nextX - archerX) / dist);
        height = 2 * (nextX - archerX) * (nextX - targetX) / (-0.25f * dist * dist);
        Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
        transform.rotation = LookAtTarget(movePosition - transform.position);
        transform.position = movePosition;
    }

    IEnumerator SpeedUp()
    {
        speed = 0.01f;
        for (int i = 1; i <= maxSpeed; i++)
        {
            yield return new WaitForSeconds(0.3f/maxSpeed);
            speed++;
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
