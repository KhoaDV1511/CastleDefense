using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float initialVelocity;
    [SerializeField] private float angleShoot;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float step;
    [SerializeField] private Transform firePoint;
    private Camera _cam;
    private Rigidbody2D _rb;

    private void Start()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector3 targetPos = _cam.ScreenToWorldPoint(Input.mousePosition) - firePoint.position;
        targetPos.z = 0;
        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(0.01f, height - 4f);
        float angle;
        float v0;
        float time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);
        //DrawPath(v0, angleShoot, time, step);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(Coroutine_Movement(v0, angle, time));
        }
    }
    
    private void TrackMovement()
    {
        Vector2 dir = _rb.velocity;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        Debug.Log(angle);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / xt);
        v0 = b / Mathf.Sin(angle);
    }

    private void CalculatePath(Vector3 targetPos, float angle, out float v0, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float v1 = Mathf.Pow(xt, 2) * g;
        float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
        float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);
        v0 = Mathf.Sqrt(v1 / (v2 - v3));
        time = xt / (v0 * Mathf.Cos(angle));
    }

    private void DrawPath(float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        line.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(i, 2);
            line.SetPosition(count, firePoint.position + new Vector3(x, y, 0));
            count++;
        }

        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(time, 2);
        line.SetPosition(count, firePoint.position + new Vector3(xFinal, yFinal, 0));
    }

    IEnumerator Coroutine_Movement(float v0, float angle, float time)
    {
        float t = 0;
        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position = firePoint.position + new Vector3(x, y, 0);
            TrackMovement();
            t += Time.deltaTime;
            yield return null;
        }
    }
}
