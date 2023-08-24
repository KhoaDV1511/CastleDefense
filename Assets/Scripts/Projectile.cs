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

    private void Awake()
    {
        Signals.Get<EnemyPosArrow>().AddListener(TrackMovement);
        if(_speedUp != null) StopCoroutine(_speedUp);
        _speedUp = StartCoroutine(SpeedUp());
    }

    private void OnDestroy()
    {
        speed = 0.01f;
        Signals.Get<EnemyPosArrow>().RemoveListener(TrackMovement);
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
        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.3f/10);
            speed++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter");
        if(other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
