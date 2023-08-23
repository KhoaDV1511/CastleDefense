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

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        TrackMovement(10);
    }

    private static Quaternion LookAtTarget(Vector2 rotation)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y - 0.01f, rotation.x - 0.01f) * Mathf.Rad2Deg);
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
    }
}
