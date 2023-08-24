using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform enemyPoint;
    private Camera _cam;
    private Rigidbody2D _rb;
    private float dist;
    private float nextX;
    private float baseY;
    private float height;
    
    private float targetX;
    private float archerX;
    private Vector3 targetPos;
    private float x;
    private float speed = 1;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(SpeedUp());
    }

    private void FixedUpdate()
    {
        // float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        TrackMovement(speed);
    }

    private static Quaternion LookAtTarget(Vector2 rotation)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
    }
    private void TrackMovement(float speed)
    {
        archerX = firePoint.position.x;
        targetX = enemyPoint.position.x;

        dist = targetX - archerX;
        nextX = Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime);
        baseY = Mathf.Lerp(firePoint.position.y, enemyPoint.position.y, (nextX - archerX) / dist);
        height = 2 * (nextX - archerX) * (nextX - targetX) / (-0.25f * dist * dist);
        Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
        transform.rotation = LookAtTarget(movePosition - transform.position);
        transform.position = movePosition;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        if(transform.position == enemyPoint.position) Destroy(gameObject);
    }

    IEnumerator SpeedUp()
    {
        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.5f/10);
            speed++;
        }
    }
}
